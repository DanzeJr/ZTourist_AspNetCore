using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZTourist.Infrastructure;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Areas.Company.Controllers
{
    [Area("Company")]
    [Authorize(Policy = "Employee")]
    public class TourController : Controller
    {
        private readonly TouristDAL touristDAL;
        private readonly UserManager<AppUser> userManager;
        private readonly BlobService blobService;

        public TourController(TouristDAL touristDAL, UserManager<AppUser> userManager, BlobService blobService)
        {
            this.touristDAL = touristDAL;
            this.userManager = userManager;
            this.blobService = blobService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            TourSearchViewModel model = new TourSearchViewModel { IsActive = true };
            model.Skip = (page - 1) * model.Fetch;
            int total = await touristDAL.GetTotalToursAsync();
            PageInfo pageInfo = new PageInfo
            {
                TotalItems = total,
                ItemPerPage = model.Fetch,
                PageAction = nameof(Index),
                CurrentPage = page
            };
            IEnumerable<Tour> tours = await touristDAL.GetAllToursAsync(model.Skip, model.Fetch);
            if (tours != null)
            {
                foreach (Tour tour in tours)
                {
                    tour.Destinations = await touristDAL.FindDestinationsByTourIdAsync(tour.Id);
                }
                model.Tours = tours;
            }
            model.PageInfo = pageInfo;
            ViewBag.Title = "All Tours";
            return View(model);
        }

        public async Task<IActionResult> Search(TourSearchViewModel model, int page = 1)
        {
            if (model == null)
                model = new TourSearchViewModel();
            model.InitSearchValues();
            model.Skip = (page - 1) * model.Fetch;
            int total = await touristDAL.GetTotalSearchToursAsync(model);
            PageInfo pageInfo = new PageInfo
            {
                TotalItems = total,
                ItemPerPage = model.Fetch,
                PageAction = nameof(Search),
                CurrentPage = page
            };
            IEnumerable<Tour> tours = await touristDAL.SearchToursAsync(model);
            if (tours != null)
            {
                foreach (Tour tour in tours)
                {
                    tour.Destinations = await touristDAL.FindDestinationsByTourIdAsync(tour.Id);
                }
                model.Tours = tours;
            }
            model.PageInfo = pageInfo;
            ViewBag.Title = "Search Tours";
            return View("Index", model);
        }

        [ImportModelState]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();
            Tour tour = await touristDAL.FindTourByTourIdEmpAsync(id);
            if (tour == null)
            {
                return NotFound();
            }
            tour.Destinations = await touristDAL.FindDestinationsByTourIdAsync(id);
            tour.Guides = await touristDAL.FindGuidesByTourIdAsync(id);
            tour.TakenSlot = await touristDAL.GetTakenSlotByTourIdAsync(id);
            return View(tour);
        }               

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Add()
        {
            TourViewModel model = new TourViewModel
            {
                Image = "https://ztourist.blob.core.windows.net/others/tour.jpg"
            };
            model.DestinationItems = await InitDestinationItemsAsync();
            model.GuideItems = await InitGuideItemsAsync();
            return View(model);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TourViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await touristDAL.IsExistedTourIdAsync(model.Id))
                {
                    ModelState.AddModelError("", $"Tour ID '{model.Id.ToUpper()}' is existed");
                }
                if (model.FromDate.CompareTo(DateTime.Now) <= 0)
                {
                    ModelState.AddModelError("", "Start Date must be greater than today");
                }
                else if (model.FromDate.CompareTo(DateTime.Parse("9999-12-31 12:00")) > 0)
                {
                    ModelState.AddModelError("", "Start Date is out of range");
                }
                if (model.ToDate.CompareTo(DateTime.Now) <= 0)
                {
                    ModelState.AddModelError("", "End Date must be greater than today");
                }
                else if (model.ToDate.CompareTo(DateTime.Parse("9999-12-31 12:00")) > 0)
                {
                    ModelState.AddModelError("", "End Dae is out of range");
                }
                if (model.FromDate.CompareTo(model.ToDate) >= 0)
                {
                    ModelState.AddModelError("", "End Date must be greater than Start Date");
                }
                if (!await touristDAL.IsAvailableDestinationAsync(model.Departure))
                {
                    ModelState.AddModelError("", $"The departure '{model.Departure.ToUpper()}' is not existed or available");
                }
                bool duplicated = false;
                foreach (string id in model.Destinations)
                {
                    if (id.Equals(model.Departure, StringComparison.OrdinalIgnoreCase))
                    {
                        duplicated = true;
                    }
                    if (!await touristDAL.IsAvailableDestinationAsync(id))
                    {
                        ModelState.AddModelError("", $"Destination '{id.ToUpper()}' is not existed or available");
                    }
                }
                if (duplicated)
                {
                    ModelState.AddModelError("", "The departure can't also be destinations");
                }
                List<string> availableGuides = new List<string>();
                IEnumerable<AppUser> guides = await userManager.GetUsersInRoleAsync("Guide");
                foreach (AppUser guide in guides)
                {
                    if (!await userManager.IsLockedOutAsync(guide))
                    {
                        availableGuides.Add(guide.Id);
                    }
                }
                foreach (string id in model.Guides)
                {
                    if (!availableGuides.Contains(id))
                    {
                        ModelState.AddModelError("", $"Guide '{id.ToUpper()}' is not existed or available");
                    }
                }
                if (!ModelState.IsValid)
                {
                    model.DestinationItems = await InitDestinationItemsAsync();
                    model.GuideItems = await InitGuideItemsAsync();
                    return View("Add", model);
                }
                model.Destinations.Insert(0, model.Departure);
                List<Destination> destinations = new List<Destination>();
                foreach (string id in model.Destinations)
                {
                    destinations.Add(new Destination { Id = id });
                }
                Tour tour = new Tour
                {
                    Id = model.Id.ToUpper(),
                    Name = model.Name,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    AdultFare = model.AdultFare,
                    KidFare = model.KidFare,
                    Description = model.Description ?? "",
                    MaxGuest = model.MaxGuest,
                    Transport = model.Transport ?? "",
                    Destinations = destinations,
                    Guides = guides.Where(g => model.Guides.Contains(g.Id)),
                    IsActive = model.IsActive
                };
                string img = model.Image ?? "https://ztourist.blob.core.windows.net/others/tour.jpg";
                if (model.Photo != null && !string.IsNullOrWhiteSpace(model.Photo?.FileName)) // if photo is chosen then copy
                {
                    string filePath = model.Id + "." + model.Photo.FileName.Substring(model.Photo.FileName.LastIndexOf(".") + 1);
                    img = await blobService.UploadFile("tours", filePath, model.Photo);
                }
                if (img != null)
                {
                    tour.Image = img;
                    if (await touristDAL.AddTourAsync(tour))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Add tour failed");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Can't upload image");
                }
            }
            model.DestinationItems = await InitDestinationItemsAsync();
            model.GuideItems = await InitGuideItemsAsync();
            return View("Add", model);
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            Tour tour = await touristDAL.FindTourByTourIdEmpAsync(id);
            if (tour == null)
            {
                return NotFound();
            }
            TourViewModel model = new TourViewModel
            {
                Id = tour.Id,
                Name = tour.Name,
                Image = tour.Image,
                Description = tour.Description,
                FromDate = tour.FromDate,
                ToDate = tour.ToDate,
                AdultFare = tour.AdultFare,
                KidFare = tour.KidFare,
                MaxGuest = tour.MaxGuest,
                Transport = tour.Transport,
                IsActive = tour.IsActive
            };

            // get related destinations and guides
            IEnumerable<Destination> destinations = await touristDAL.FindDestinationsByTourIdAsync(id);
            IEnumerable<AppUser> guides = await touristDAL.FindGuidesByTourIdAsync(id);
            if (destinations != null)
            {
                model.Destinations = destinations.Select(d => d.Id).ToList<string>();
                model.Departure = model.Destinations[0];
                model.Destinations.RemoveAt(0);
            }

            if (guides != null)
                model.Guides = guides.Select(g => g.Id).ToList<string>();

            model.DestinationItems = await InitDestinationItemsAsync();
            model.GuideItems = await InitGuideItemsAsync();
            return View(model);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(TourViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!await touristDAL.IsExistedTourIdAsync(model.Id))
                {
                    return NotFound();
                }
                if (model.FromDate.CompareTo(DateTime.Now) <= 0)
                {
                    ModelState.AddModelError("", "Start Date must be greater than today");
                }
                else if (model.FromDate.CompareTo(DateTime.Parse("9999-12-31 12:00")) > 0)
                {
                    ModelState.AddModelError("", "Start Date is out of range");
                }
                if (model.ToDate.CompareTo(DateTime.Now) <= 0)
                {
                    ModelState.AddModelError("", "End Date must be greater than today");
                }
                else if (model.ToDate.CompareTo(DateTime.Parse("9999-12-31 12:00")) > 0)
                {
                    ModelState.AddModelError("", "End Dae is out of range");
                }
                if (model.FromDate.CompareTo(model.ToDate) >= 0)
                {
                    ModelState.AddModelError("", "End Date must be greater than Start Date");
                }
                if (!await touristDAL.IsAvailableDestinationAsync(model.Departure))
                {
                    ModelState.AddModelError("", $"Departure '{model.Departure.ToUpper()}' is not existed or available");
                }
                bool duplicated = false;
                foreach (string id in model.Destinations)
                {
                    if (id.Equals(model.Departure, StringComparison.OrdinalIgnoreCase))
                    {
                        duplicated = true;
                    }
                    if (!await touristDAL.IsAvailableDestinationAsync(id))
                    {
                        ModelState.AddModelError("", $"Destination '{id.ToUpper()}' is not existed or available");
                    }
                }
                if (duplicated)
                {
                    ModelState.AddModelError("", "The departure can't also be destinations");
                }
                List<string> availableGuides = new List<string>();
                IEnumerable<AppUser> guides = await userManager.GetUsersInRoleAsync("Guide");
                foreach (AppUser guide in guides)
                {
                    if (!await userManager.IsLockedOutAsync(guide))
                    {
                        availableGuides.Add(guide.Id);
                    }
                }
                foreach (string id in model.Guides)
                {
                    if (!availableGuides.Contains(id))
                    {
                        ModelState.AddModelError("", $"Guide '{id.ToUpper()}' is not existed or available");
                    }
                }
                if (!ModelState.IsValid)
                {
                    model.DestinationItems = await InitDestinationItemsAsync();
                    model.GuideItems = await InitGuideItemsAsync();
                    return View("Edit", model);
                }
                model.Destinations.Insert(0, model.Departure);
                List<Destination> destinations = new List<Destination>();
                foreach (string id in model.Destinations)
                {
                    destinations.Add(new Destination { Id = id });
                }
                Tour tour = new Tour
                {
                    Id = model.Id,
                    Name = model.Name,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    AdultFare = model.AdultFare,
                    KidFare = model.KidFare,
                    Description = model.Description ?? "",
                    MaxGuest = model.MaxGuest,
                    Transport = model.Transport ?? "",
                    Destinations = destinations,
                    Guides = guides.Where(g => model.Guides.Contains(g.Id)),
                    IsActive = model.IsActive
                };
                string img;
                if (model.Photo != null && !string.IsNullOrWhiteSpace(model.Photo?.FileName)) // if photo is change then copy
                {
                    string filePath = model.Id + "." + model.Photo.FileName.Substring(model.Photo.FileName.LastIndexOf(".") + 1);
                    img = await blobService.UploadFile("tours", filePath, model.Photo);
                }
                else // if not, preserve old one
                    img = model.Image;
                if (img != null)
                {
                    tour.Image = img;
                    if (await touristDAL.UpdateTourAsync(tour))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Update tour failed");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Can't upload image");
                }
            }
            model.DestinationItems = await InitDestinationItemsAsync();
            model.GuideItems = await InitGuideItemsAsync();
            return View("Edit", model);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Activate(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !await touristDAL.IsExistedTourIdAsync(id))
            {
                return NotFound();
            }
            if (!await touristDAL.UpdateStatusTourByIdAsync(id, true))
            {
                ModelState.AddModelError("", "Activate tour failed");
            }
            return RedirectToAction(nameof(Details), new { Id = id });
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Deactivate(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !await touristDAL.IsExistedTourIdAsync(id))
            {
                return NotFound();
            }
            if (!await touristDAL.UpdateStatusTourByIdAsync(id, false))
            {
                ModelState.AddModelError("", "Deactivate tour failed");
            }
            return RedirectToAction(nameof(Details), new { Id = id });
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !await touristDAL.IsExistedTourIdAsync(id))
            {
                return NotFound();
            }
            if (!await touristDAL.HasOrderByTourIdAsync(id))
            {
                if (!await touristDAL.DeleteTourByIdAsync(id))
                {
                    ModelState.AddModelError("", "Delete tour failed");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                ModelState.AddModelError("", "Delete failed\nTour already has orders");
            }
            return RedirectToAction(nameof(Details), new { Id = id });
        }

        public async Task<IActionResult> IsExistedId(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (await touristDAL.IsExistedTourIdAsync(id))
                {
                    return Json($"Tour ID '{id}' is already existed");
                }
            }
            return Json(true);
        }

        [NonAction]
        private async Task<SelectList> InitDestinationItemsAsync()
        {
            Dictionary<string, string> destinationItems = await touristDAL.GetDestinationsIdNameAsync();
            return new SelectList(destinationItems, "Key", "Value");
        }

        [NonAction]
        private async Task<SelectList> InitGuideItemsAsync()
        {
            Dictionary<string, string> guideItems = new Dictionary<string, string>();
            IEnumerable<AppUser> users = await userManager.GetUsersInRoleAsync("Guide");
            foreach (AppUser user in users)
            {
                if (!await userManager.IsLockedOutAsync(user))
                {
                    guideItems.Add(user.Id, user.FirstName + " " + user.LastName + " (" + user.NormalizedUserName + ")");
                }
            }
            return new SelectList(guideItems, "Key", "Value");
        }
    }
}
