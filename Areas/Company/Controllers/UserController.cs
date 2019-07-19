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
        private readonly BlobService blobService;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, TouristDAL touristDAL, BlobService blobService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.touristDAL = touristDAL;
            this.blobService = blobService;
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
            UserViewModel model = new UserViewModel
            {
                Profile = user,
                Roles = await userManager.GetRolesAsync(user)
            };
            return View(model);
        }

        public IActionResult Add()
        {
            UserCreateModel model = new UserCreateModel
            {
                Avatar = "https://ztourist.blob.core.windows.net/others/avatar.png",
                BirthDate = DateTime.Now,
                RoleItems = new List<SelectListItem>()
            };
            foreach (IdentityRole role in roleManager.Roles)
            {
                model.RoleItems.Add(new SelectListItem(role.Name, role.Name));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(UserCreateModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Roles.Contains("Customer") && model.Roles.Count() > 1) // if user is customer and also some roles, invalid
                {
                    ModelState.AddModelError("", "Not allowed to assign other roles to customer");
                }
                else
                {
                    AppUser user = new AppUser
                    {
                        UserName = model.UserName,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Address = model.Address,
                        Email = model.Email,
                        BirthDate = model.BirthDate,
                        Gender = model.Gender,
                        PhoneNumber = model.Tel,
                        Avatar = model.Avatar,
                        RegisterDate = DateTime.Now
                    };
                    if (model.IsLocked)
                    {
                        user.LockoutEnd = DateTimeOffset.MaxValue;
                    }
                    else
                    {
                        user.LockoutEnd = null;
                    }
                    string avatar;
                    if (model.Photo != null && !string.IsNullOrWhiteSpace(model.Photo.FileName)) // if photo is change then copy
                    {
                        string filePath = user.UserName + "." + model.Photo.FileName.Substring(model.Photo.FileName.LastIndexOf(".") + 1);
                        avatar = await blobService.UploadFile("avatars", filePath, model.Photo);
                    }
                    else // if not, preserve old one
                        avatar = user.Avatar;
                    if (avatar != null)
                    {
                        user.Avatar = avatar;
                        IdentityResult result = await userManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            result = await userManager.AddToRolesAsync(user, model.Roles);
                            if (!result.Succeeded)
                            {
                                AddErrorFromResult(result);
                            }
                            if (ModelState.IsValid) // if everything is ok
                                return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            AddErrorFromResult(result);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Can't upload avatar");
                    }
                }
            }
            model.RoleItems = new List<SelectListItem>();
            foreach (IdentityRole role in roleManager.Roles)
            {
                model.RoleItems.Add(new SelectListItem(role.Name, role.Name));
            }
            return View("Add", model);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserEditModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Roles.Contains("Customer") && model.Roles.Count() > 1) // if user is customer and also some roles, invalid
                {
                    ModelState.AddModelError("", "Not allowed to assign other roles to customer");
                }
                else
                {
                    AppUser user = await userManager.FindByNameAsync(model.UserName);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    // if existed
                    IEnumerable<string> roles = await userManager.GetRolesAsync(user);
                    bool notChanged = model.Roles.OrderBy(r => r).SequenceEqual(roles.OrderBy(r => r)); // compare to determine if role has changed
                    if (!notChanged) // if role is change
                    {
                        if (model.UserName.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase) && !model.Roles.Contains("Admin")) // if current user is try to remove role admin from his/her account
                        {
                            ModelState.AddModelError("", "You are not allowed to remove 'Admin' role from your account");
                        }
                        else if (roles.Contains("Customer") && !model.Roles.Contains("Customer")) //if user is a customer and updating remove customer role from user
                        {
                            ModelState.AddModelError("", "Customer is not allowed to change role");
                        }

                        if (roles.Contains("Guide")) //if user is a guide
                        {
                            if (!model.Roles.Contains("Guide"))  // if updating remove guide role from user
                            {
                                IEnumerable<string> tours = await touristDAL.FindToursByUserIdAsync(user.Id);
                                if (tours != null) // if guide is used in any tours then it can't be remove
                                    ModelState.AddModelError("", $"Can't remove 'Guide' role. This guide is used in tours: {string.Join(", ", tours)}");
                            }
                            if (model.IsLocked) // if want to lock this guide
                            {
                                IEnumerable<string> tours = await touristDAL.FindFutureToursByUserIdAsync(user.Id);
                                if (tours != null)
                                    ModelState.AddModelError("", $"Can't lock this user. Remove this guide from the following tours before lock: {string.Join(", ", tours)}");
                            }
                        }
                        if (!ModelState.IsValid)
                        {
                            model.RoleItems = new List<SelectListItem>();
                            foreach (IdentityRole role in roleManager.Roles)
                            {
                                model.RoleItems.Add(new SelectListItem(role.Name, role.Name));
                            }
                            return View("Edit", model);
                        }
                    }
                    bool isSameEmail = user.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase);
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.Email = model.Email;
                    user.BirthDate = model.BirthDate;
                    user.Gender = model.Gender;
                    user.PhoneNumber = model.Tel;
                    bool changedStatus = false;
                    if (model.IsLocked)
                    {
                        if (user.LockoutEnd == null)
                        {
                            changedStatus = true;
                        }
                        user.LockoutEnd = DateTimeOffset.MaxValue;
                    }
                    else
                    {
                        if (user.LockoutEnd != null)
                        {
                            changedStatus = true;
                        }
                        user.LockoutEnd = null;
                    }
                    string avatar;
                    if (model.Photo != null && !string.IsNullOrWhiteSpace(model.Photo.FileName)) // if photo is change then copy
                    {
                        string filePath = user.UserName + "." + model.Photo.FileName.Substring(model.Photo.FileName.LastIndexOf(".") + 1);
                        avatar = await blobService.UploadFile("avatars", filePath, model.Photo);
                    }
                    else // if not, preserve old one
                        avatar = user.Avatar;
                    if (avatar != null)
                    {
                        user.Avatar = avatar;
                        IdentityResult result = null;
                        if (isSameEmail)
                        {
                            IEnumerable<UserLoginInfo> loginInfos = await userManager.GetLoginsAsync(user);
                            foreach (UserLoginInfo info in loginInfos)
                            {
                                result = await userManager.RemoveLoginAsync(user, info.LoginProvider, info.ProviderKey);
                            }
                        }
                        if (result == null || result.Succeeded) // if don't need to remove external login or remove external login successfully
                        {
                            result = await userManager.UpdateAsync(user);
                            if (result.Succeeded)
                            {

                                if (changedStatus || !notChanged)
                                {
                                    if (!notChanged)
                                    {
                                        result = await userManager.AddToRolesAsync(user, model.Roles.Except(roles));
                                        if (!result.Succeeded)
                                        {
                                            AddErrorFromResult(result);
                                        }
                                        result = await userManager.RemoveFromRolesAsync(user, roles.Except(model.Roles));
                                        if (!result.Succeeded)
                                        {
                                            AddErrorFromResult(result);
                                        }
                                    }
                                    await userManager.UpdateSecurityStampAsync(user);
                                }
                                if (ModelState.IsValid) // if everything is ok
                                    return RedirectToAction(nameof(Details), new { userName = model.UserName });
                            }
                            else // if update failed
                            {
                                AddErrorFromResult(result);
                            }
                        }
                        else // if remove external login failed
                        {
                            AddErrorFromResult(result);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Can't upload avatar");
                    }
                }
            }
            model.RoleItems = new List<SelectListItem>();
            foreach (IdentityRole role in roleManager.Roles)
            {
                model.RoleItems.Add(new SelectListItem(role.Name, role.Name));
            }
            return View("Edit", model);
        }

        [NonAction]
        private void AddErrorFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
