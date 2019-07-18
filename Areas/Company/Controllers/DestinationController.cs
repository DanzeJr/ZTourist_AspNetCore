using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZTourist.Infrastructure;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Areas.Company.Controllers
{
    [Area("Company")]
    [Authorize(Policy = "Employee")]
    public class DestinationController : Controller
    {
        private readonly TouristDAL touristDAL;
        private readonly BlobService blobService;

        public DestinationController(TouristDAL touristDAL, BlobService blobService)
        {
            this.touristDAL = touristDAL;
            this.blobService = blobService;
        }

        public async Task<IActionResult> Index(bool? isActive, int page = 1)
        {
            DestinationSearchViewModel model = new DestinationSearchViewModel { IsActive = isActive };
            model.Skip = (page - 1) * model.Fetch;
            int total = await touristDAL.GetTotalDestinationsAsync(model.IsActive);
            PageInfo pageInfo = new PageInfo
            {
                TotalItems = total,
                ItemPerPage = model.Fetch,
                PageAction = nameof(Index),
                CurrentPage = page
            };
            model.Destinations = await touristDAL.GetAllDestinationsAsync(model.IsActive, model.Skip, model.Fetch);
            model.PageInfo = pageInfo;
            if (isActive == null)
                ViewBag.Title = "All Destinations";
            else if (isActive == true)
                ViewBag.Title = "Active Destinations";
            else
                ViewBag.Title = "Not Active Destinations";
            return View(model);
        }

        [ImportModelState]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();
            Destination destination = await touristDAL.FindDestinationByIdAsync(id);
            if (destination == null)
            {
                return NotFound();
            }
            return View(destination);
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Add()
        {
            DestinationViewModel model = new DestinationViewModel
            {
                Image = "https://ztourist.blob.core.windows.net/others/destination.jpg"
            };
            return View(model);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(DestinationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await touristDAL.IsExistedDestinationIdAsync(model.Id))
                {
                    ModelState.AddModelError("", $"Destination ID '{model.Id.ToUpper()}' is existed");
                }
                if (!ModelState.IsValid)
                {
                    return View("Add", model);
                }
                Destination destination = new Destination
                {
                    Id = model.Id.ToUpper(),
                    Name = model.Name,
                    Description = model.Description ?? "",
                    Country = model.Country,
                    IsActive = model.IsActive
                };
                string img = model.Image ?? "https://ztourist.blob.core.windows.net/others/destination.jpg";
                if (model.Photo != null && !string.IsNullOrWhiteSpace(model.Photo?.FileName)) // if photo is chosen then copy
                {
                    string filePath = model.Id + "." + model.Photo.FileName.Substring(model.Photo.FileName.LastIndexOf(".") + 1);
                    img = await blobService.UploadFile("destinations", filePath, model.Photo);
                }
                if (img != null)
                {
                    destination.Image = img;
                    if (await touristDAL.AddDestinationAsync(destination))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Add destination failed");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Can't upload image");
                }
            }
            return View("Add", model);
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            Destination destination = await touristDAL.FindDestinationByIdAsync(id);
            if (destination == null)
            {
                return NotFound();
            }
            DestinationViewModel model = new DestinationViewModel
            {
                Id = destination.Id,
                Name = destination.Name,
                Image = destination.Image,
                Description = destination.Description ?? "",
                Country = destination.Country,
                IsActive = destination.IsActive
            };
            return View(model);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(DestinationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!await touristDAL.IsExistedDestinationIdAsync(model.Id))
                {
                    return NotFound();
                }
                Destination destination = new Destination
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description ?? "",
                    Country = model.Country,
                    IsActive = model.IsActive
                };
                string img = model.Image ?? "https://ztourist.blob.core.windows.net/others/destination.jpg";
                if (model.Photo != null && !string.IsNullOrWhiteSpace(model.Photo?.FileName)) // if photo is chosen then copy
                {
                    string filePath = model.Id + "." + model.Photo.FileName.Substring(model.Photo.FileName.LastIndexOf(".") + 1);
                    img = await blobService.UploadFile("destinations", filePath, model.Photo);
                }
                if (img != null)
                {
                    destination.Image = img;
                    IEnumerable<string> tours = await touristDAL.FindFutureToursByDestinationIdAsync(model.Id);
                    if (tours != null)
                    {
                        ModelState.AddModelError("", $"Destination is using in tours: {string.Join(", ", tours)}\nPlease remove this destination from these tours first");
                    }
                    else
                    {
                        if (await touristDAL.UpdateDestinationAsync(destination))
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ModelState.AddModelError("", "Update destination failed");
                        }
                    }
                    
                }
                else
                {
                    ModelState.AddModelError("", "Can't upload image");
                }
            }
            return View("Edit", model);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !await touristDAL.IsExistedDestinationIdAsync(id))
            {
                return NotFound();
            }
            IEnumerable<string> tours = await touristDAL.GetToursByDestinationIdAsync(id);
            if (tours == null)
            {
                if (!await touristDAL.DeleteDestinationByIdAsync(id))
                {
                    ModelState.AddModelError("", $"Delete destination failed");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else // if destination is used
            {
                ModelState.AddModelError("", $"Destination is used in tours: {string.Join(",", tours)}\nPlease remove this destination from these tours to delete");
            }
            return RedirectToAction(nameof(Details), new { Id = id });
        }

        public async Task<IActionResult> IsExistedId(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (await touristDAL.IsExistedDestinationIdAsync(id))
                {
                    return Json($"Destination ID '{id}' is already existed");
                }
            }
            return Json(true);
        }
    }
}
