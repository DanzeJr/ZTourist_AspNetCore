using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZTourist.Infrastructure;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Areas.Company.Controllers
{
    [Area("Company")]
    [Authorize(Policy = "NotEmployee")]
    public class TourController : Controller
    {
        private readonly TouristDAL touristDAL;

        public TourController(TouristDAL touristDAL)
        {
            this.touristDAL = touristDAL;
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
                    tour.Destinations = await touristDAL.GetDestinationsByTourIdAsync(tour.Id);
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
                    tour.Destinations = await touristDAL.GetDestinationsByTourIdAsync(tour.Id);
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
            Tour tour = await touristDAL.FindTourByTourIdAsync(id);
            if (tour == null)
            {
                return NotFound();
            }
            else
            {
                tour.Destinations = await touristDAL.FindDestinationsByTourIdAsync(id);
                tour.TakenSlot = await touristDAL.GetTakenSlotByTourIdAsync(id);
            }                
            CartLine model = new CartLine { Tour = tour };
            return View(model);
        }

    }
}
