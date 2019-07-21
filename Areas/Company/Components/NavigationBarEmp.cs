using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;
using ZTourist.Models;
using ZTourist.Models.ViewModels;
using ILogger = Serilog.ILogger;

namespace ZTourist.Areas.Company.Components
{
    public class NavigationBarEmp : ViewComponent
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger logger;

        public NavigationBarEmp(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.logger = new LoggerConfiguration()
                .WriteTo.AzureBlobStorage(configuration["Data:StorageAccount"], Serilog.Events.LogEventLevel.Information, $"logs", "{yyyy}/{MM}/{dd}/log.txt").CreateLogger();
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                AppUser user = User?.Identity?.Name == null ? null : await userManager.FindByNameAsync(User.Identity.Name);
                string avatar = "https://ztourist.blob.core.windows.net/others/avatar.png";
                if (user != null)
                    avatar = user.Avatar;
                NavigationViewModel model = new NavigationViewModel
                {
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
