using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZTourist.Infrastructure;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Controllers
{
    [Authorize(Policy = "NotEmployee")]
    public class DestinationController : Controller
    {
        private readonly DestinationDAL destinationDAL;

        public DestinationController(DestinationDAL destinationDAL)
        {
            this.destinationDAL = destinationDAL;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            DestinationSearchViewModel model = new DestinationSearchViewModel { IsActive = true };
            model.Skip = (page - 1) * model.Fetch;
            int total = await destinationDAL.GetTotalDestinationsAsync(model.IsActive);
            PageInfo pageInfo = new PageInfo
            {
                TotalItems = total,
                ItemPerPage = model.Fetch,
                PageAction = nameof(Index),
                CurrentPage = page
            };
            model.Destinations = await destinationDAL.GetAllDestinationsAsync(model.IsActive, model.Skip, model.Fetch);
            model.PageInfo = pageInfo;
            ViewBag.Title = "All Destinations";
            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();
            Destination destination = await destinationDAL.FindDestinationByIdAsync(id, true);
            if (destination == null)
            {
                return NotFound();
            }
            return View(destination);
        }

    }
}
