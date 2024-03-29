﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZTourist.Models;
using ZTourist.Models.ViewModels;
using ILogger = Serilog.ILogger;

namespace ZTourist.Components
{
    public class Slider : ViewComponent
    {
        private readonly TourDAL tourDAL;
        private readonly DestinationDAL destinationDAL;
        private readonly ILogger logger;

        public Slider(TourDAL tourDAL, DestinationDAL destinationDAL, IConfiguration configuration)
        {
            this.tourDAL = tourDAL;
            this.destinationDAL = destinationDAL;
            this.logger = new LoggerConfiguration()
                .WriteTo.AzureBlobStorage(configuration["Data:StorageAccount"], Serilog.Events.LogEventLevel.Information, $"logs", "{yyyy}/{MM}/{dd}/log.txt").CreateLogger();
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                TourSearchViewModel model = new TourSearchViewModel();
                IEnumerable<Tour> tours = await tourDAL.GetNearestToursAsync();
                if (tours != null)
                {
                    foreach (Tour tour in tours)
                    {
                        tour.Destinations = await tourDAL.FindDestinationsByTourIdAsync(tour.Id);
                    }
                    model.Tours = tours;
                }
                Dictionary<string, string> destinations = await destinationDAL.GetDestinationsIdNameAsync();
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
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }            
        }
    }
}
