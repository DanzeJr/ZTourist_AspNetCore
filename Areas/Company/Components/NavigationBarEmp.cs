using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public NavigationBarEmp(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            AppUser user = User?.Identity?.Name == null ? null : await userManager.FindByNameAsync(User.Identity.Name);
            string avatar = "https://ztourist.blob.core.windows.net/others/avatar.png";
            if (user != null)
                avatar = user.Avatar;
            NavigationViewModel model = new NavigationViewModel {
                Avatar = avatar
            };
            return View(model);
        }
    }
}
