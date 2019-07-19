using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ZTourist.Infrastructure;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Areas.Company.Controllers
{
    [Area("Company")]
    [Authorize(Policy = "Employee")]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly BlobService blobService;

        public ProfileController(UserManager<AppUser> userManager, BlobService blobService)
        {
            this.userManager = userManager;
            this.blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            AppUser user = await userManager.FindByNameAsync(User.Identity.Name);
            return View(user);
        }

        [ImportModelState]
        public async Task<IActionResult> Edit()
        {
            AppUser user = await userManager.FindByNameAsync(User.Identity.Name);
            ProfileModel model = new ProfileModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Email = user.Email,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                Tel = user.PhoneNumber,
                Avatar = user.Avatar
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Update(ProfileModel profile)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(User.Identity.Name);
                bool isSameEmail = user.Email.Equals(profile.Email, StringComparison.OrdinalIgnoreCase);
                user.FirstName = profile.FirstName;
                user.LastName = profile.LastName;
                user.Address = profile.Address;
                user.Email = profile.Email;
                user.BirthDate = profile.BirthDate;
                user.Gender = profile.Gender;
                user.PhoneNumber = profile.Tel;
                string avatar;
                if (profile.Photo != null && !string.IsNullOrWhiteSpace(profile.Photo.FileName)) // if photo is change then copy
                {
                    string filePath = user.UserName + "." + profile.Photo.FileName.Substring(profile.Photo.FileName.LastIndexOf(".") + 1);
                    avatar = await blobService.UploadFile("avatars", filePath, profile.Photo);
                }
                else // if not, preserve old one
                    avatar = user.Avatar;
                if (avatar != null)
                {
                    user.Avatar = avatar;
                    IdentityResult result = null;
                    if (!isSameEmail) // remove external login when email change
                    {
                        IEnumerable<UserLoginInfo> loginInfos = await userManager.GetLoginsAsync(user);
                        foreach (UserLoginInfo info in loginInfos)
                        {
                            await userManager.RemoveLoginAsync(user, info.LoginProvider, info.ProviderKey);
                        }
                    }
                    if (result == null || result.Succeeded)
                    {
                        result = await userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }                    
                    AddErrorFromResult(result);
                }
                else
                {
                    ModelState.AddModelError("", "Can't upload avatar");
                }
            }
            return RedirectToAction(nameof(Edit));
        }

        [ImportModelState]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> ChangePassword(PasswordModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    ModelState.AddModelError("", "Your username is not found");
                }
                else
                {
                    IdentityResult result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    AddErrorFromResult(result);
                }
            }
            return RedirectToAction(nameof(ChangePassword));
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
