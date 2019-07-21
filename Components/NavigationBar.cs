using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class NavigationBar : ViewComponent
    {
        private readonly TourDAL tourDAL;
        private readonly UserManager<AppUser> userManager;
        private readonly Cart cart;
        private readonly ILogger logger;

        public NavigationBar(TourDAL tourDAL, UserManager<AppUser> userManager, Cart cart, IConfiguration configuration)
        {
            this.tourDAL = tourDAL;
            this.userManager = userManager;
            this.cart = cart;
            this.logger = new LoggerConfiguration()
                .WriteTo.AzureBlobStorage(configuration["Data:StorageAccount"], Serilog.Events.LogEventLevel.Information, $"logs", "{yyyy}/{MM}/{dd}/log.txt").CreateLogger();
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                IEnumerable<Tour> popularTours = await tourDAL.GetTrendingToursAsync();
                if (popularTours != null)
                {
                    foreach (Tour tour in popularTours)
                    {
                        tour.Destinations = await tourDAL.FindDestinationsByTourIdAsync(tour.Id);
                    }
                }
                AppUser user = User?.Identity?.Name == null ? null : await userManager.FindByNameAsync(User.Identity.Name);
                string avatar = "https://ztourist.blob.core.windows.net/others/avatar.png";
                if (user != null)
                    avatar = user.Avatar;

                if (cart != null && cart.Lines.Count() > 0)
                {
                    foreach (CartLine cartLine in cart.Lines)
                    {
                        cartLine.Tour = await tourDAL.FindTourByTourIdAsync(cartLine.Tour.Id);
                    }
                }
                NavigationViewModel model = new NavigationViewModel
                {
                    Cart = cart,
                    PopularTours = popularTours,
                    Avatar = avatar
                };
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
