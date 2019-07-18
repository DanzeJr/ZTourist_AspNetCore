using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZTourist.Areas.Company.Models.ViewModels;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZTourist.Areas.Company.Controllers
{
    [Area("Company")]
    [Authorize(Policy = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly TouristDAL touristDAL;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, TouristDAL touristDAL)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.touristDAL = touristDAL;
        }

        public async Task<IActionResult> Index(bool? isLocked = null, int page = 1)
        {
            UserSearchViewModel model = new UserSearchViewModel { Users = new List<UserViewModel>(), IsLocked = isLocked };
            model.Skip = (page - 1) * model.Fetch;
            int total;
            IEnumerable<AppUser> appUsers;
            if (isLocked == null)
            {
                ViewBag.Title = "All Users";
                total = userManager.Users.Count();
                appUsers = userManager.Users.Skip(model.Skip).Take(model.Fetch);
            }
            else if (isLocked == false)
            {
                ViewBag.Title = "Search Users";
                total = userManager.Users.Where(u => (u.LockoutEnabled && u.LockoutEnd == null)).Count();
                appUsers = userManager.Users.Where(u => (u.LockoutEnabled && u.LockoutEnd == null)).Skip(model.Skip).Take(model.Fetch);
            }
            else
            {
                ViewBag.Title = "Locked Users";
                total = userManager.Users.Where(u => (u.LockoutEnabled && u.LockoutEnd != null)).Count();
                appUsers = userManager.Users.Where(u => (u.LockoutEnabled && u.LockoutEnd != null)).Skip(model.Skip).Take(model.Fetch);
            }
            if (appUsers != null)
            {
                IEnumerable<string> roles;
                foreach (AppUser user in appUsers)
                {
                    roles = await userManager.GetRolesAsync(user);
                    model.Users.Add(new UserViewModel { Profile = user, Roles = roles });
                }
            }
            PageInfo pageInfo = new PageInfo
            {
                TotalItems = total,
                ItemPerPage = model.Fetch,
                PageAction = nameof(Index),
                CurrentPage = page
            };
            model.PageInfo = pageInfo;
            return View(model);
        }

        public async Task<IActionResult> Details(string userName)
        {
            AppUser user;
            if (string.IsNullOrWhiteSpace(userName) || (user = await userManager.FindByNameAsync(userName)) == null)
            {
                return NotFound();
            }
            UserViewModel model = new UserViewModel {
                Profile = user,
                Roles = await userManager.GetRolesAsync(user)
            };
            return View(model);
        }

        public async Task<IActionResult> Edit(string userName)
        {
            AppUser user;
            if (string.IsNullOrWhiteSpace(userName) || (user = await userManager.FindByNameAsync(userName)) == null)
            {
                return NotFound();
            }
            UserEditModel model = new UserEditModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Address = user.Address,
                Email = user.Email,
                Tel = user.PhoneNumber,
                BirthDate = user.BirthDate,
                Avatar = user.Avatar,
                IsLocked = user.LockoutEnd != null                
            };
            model.Roles = await userManager.GetRolesAsync(user);
            model.RoleItems = new List<SelectListItem>();
            foreach (IdentityRole role in roleManager.Roles)
            {
                model.RoleItems.Add(new SelectListItem(role.Name, role.Name));
            }
            return View(model);
        }
    }
}
