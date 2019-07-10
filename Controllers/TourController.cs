using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Controllers
{
    public class TourController : Controller
    {
        private readonly TouristDAL touristDAL;

        public TourController(TouristDAL touristDAL)
        {
            this.touristDAL = touristDAL;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            TourViewModel model = new TourViewModel { IsActive = true };
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
            model.Title = "All Tours";
            return View(model);
        }

        public async Task<IActionResult> Search(TourViewModel tvm, int page = 1)
        {
            if (tvm == null)
                tvm = new TourViewModel();
            tvm.InitSearchValues();
            if (!(User.Identity.IsAuthenticated && User.IsInRole("Admin")))
                tvm.IsActive = true;
            tvm.Skip = (page - 1) * tvm.Fetch;
            int total = await touristDAL.GetTotalSearchToursAsync(tvm);
            PageInfo pageInfo = new PageInfo
            {
                TotalItems = total,
                ItemPerPage = tvm.Fetch,
                PageAction = nameof(Search),
                CurrentPage = page
            };
            IEnumerable<Tour> tours = await touristDAL.SearchToursAsync(tvm);
            if (tours != null)
            {
                foreach (Tour tour in tours)
                {
                    tour.Destinations = await touristDAL.GetDestinationsByTourIdAsync(tour.Id);
                }
                tvm.Tours = tours;
            }
            tvm.PageInfo = pageInfo;
            tvm.Title = "Search Tours";
            return View("Index", tvm);
        }

        public async Task<IActionResult> Details(string id)
        {
            Tour tour = await touristDAL.FindTourByTourIdAsync(id);
            if (tour != null)
                tour.Destinations = await touristDAL.FindDestinationsByTourIdAsync(id);
            CartLine model = new CartLine { Tour = tour };
            return View(model);
        }

    }
}
