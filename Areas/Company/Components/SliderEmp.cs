using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Areas.Company.Components
{
    public class SliderEmp : ViewComponent
    {
        private readonly TouristDAL touristDAL;

        public SliderEmp(TouristDAL touristDAL)
        {
            this.touristDAL = touristDAL;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            TourSearchViewModel model = new TourSearchViewModel();
            IEnumerable<Tour> tours = await touristDAL.GetNearestToursAsync();
            if (tours != null)
            {
                foreach (Tour tour in tours)
                {
                    tour.Destinations = await touristDAL.FindDestinationsByTourIdAsync(tour.Id);
                }
                model.Tours = tours;
            }
            Dictionary<string, string> destinations = await touristDAL.GetDestinationsIdNameAsync();
            model.DurationItems = new List<SelectListItem>
            {
                new SelectListItem { Text = "Below or in 2 Days", Value = "2" },
                new SelectListItem { Text = "Below or in 5 Days", Value = "5" },
                new SelectListItem { Text = "Below or in 1 Week", Value = "7" },
                new SelectListItem { Text = "More than 1 week", Value = "8" }
            };
            model.PriceItems = new List<SelectListItem>
            {
                new SelectListItem { Text = "Below 50$", Value = "1" },
                new SelectListItem { Text = "50$ - 250$", Value = "2" },
                new SelectListItem { Text = "250$ - 500$", Value = "3" },
                new SelectListItem { Text = "500$ - 1000$", Value = "4" },
                new SelectListItem { Text = "1000$ - 1500$", Value = "5" },
                new SelectListItem { Text = "1500$ - 2000$", Value = "6" },
                new SelectListItem { Text = "2000$ - 2500$", Value = "7" },
                new SelectListItem { Text = "Upper 2500$", Value = "8" }
            };
            if (destinations != null)
                model.DestinationItems = new SelectList(destinations, "Key", "Value");
            return View(model);
        }
    }
}
