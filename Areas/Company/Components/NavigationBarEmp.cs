using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Areas.Company.Components
{
    public class NavigationBarEmp : ViewComponent
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger logger;

        public NavigationBarEmp(UserManager<AppUser> userManager, ILogger logger)
        {
            this.userManager = userManager;
            this.logger = logger;
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
                logger.LogError(ex.Message);
                throw;
            }            
        }
    }
}
