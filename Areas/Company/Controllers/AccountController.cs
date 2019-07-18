using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZTourist.Infrastructure;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Areas.Company.Controllers
{
    [Area("Company")]
    [Authorize(Policy = "NotCustomer")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [ImportModelState]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(login.UserName);
                if (user != null && (await userManager.IsInRoleAsync(user, "Admin") || await userManager.IsInRoleAsync(user, "Guide"))) // if user's role is admin or guide
                {
                    if (await userManager.IsLockedOutAsync(user))
                    {
                        ModelState.AddModelError("", "Your account has been locked");
                        return RedirectToAction(nameof(Login), new { returnUrl = login.ReturnUrl });
                    }
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, login.Password, false, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(login.ReturnUrl) && Url.IsLocalUrl(login.ReturnUrl))
                            return Redirect(login.ReturnUrl);
                        else
                            return RedirectToAction("", "Home");
                    }
                }
                // if user is not existed or is admin or guide
                ModelState.AddModelError("", "Invalid User or Password");
            }
            return RedirectToAction(nameof(Login), new { returnUrl = login.ReturnUrl });
        }
        
        public IActionResult GoogleLogin(string returnUrl)
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account",
            new { ReturnUrl = returnUrl });
            var properties = signInManager
            .ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }
            var result = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }
            else
            {
                AppUser user = new AppUser
                {
                    Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
                };
                IdentityResult identResult = await userManager.CreateAsync(user);
                if (identResult.Succeeded)
                {
                    identResult = await userManager.AddLoginAsync(user, info);
                    if (identResult.Succeeded)
                    {
                        await signInManager.SignInAsync(user, false);
                        return Redirect(returnUrl);
                    }
                }
                return AccessDenied();
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> IsExistedUsername(string username)
        {
            if (username == null)
            {
                username = HttpContext.Request.Query["SignUpModel.UserName"];
            }
            if (!string.IsNullOrWhiteSpace(username))
                if (await userManager.FindByNameAsync(username) != null)
                {
                    return Json($"User name '{username}' is already taken");
                }
            return Json(true);
        }

        public async Task<IActionResult> IsExistedEmail(string email, string username)
        {
            if (email == null) // if param name is not 'email'
            {
                email = HttpContext.Request.Query["SignUpModel.Email"]; // if param is from sign up
                if (email == null)
                    email = HttpContext.Request.Query["ProfileModel.Email"]; // if param is from edit profile
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                AppUser user = await userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(username)) // if username param is pass to action
                    {
                        if (username.Equals(user.UserName, StringComparison.OrdinalIgnoreCase)) // if username is the same then this is email of the user with this username
                        {
                            return Json(true);
                        }
                    }
                    if (User?.Identity?.Name != null)
                    {
                        if (User.Identity.Name.Equals(user.UserName, StringComparison.OrdinalIgnoreCase)) // if email is requester's email
                            return Json(true);
                    }
                    return Json($"Email '{email}' is already taken");
                }
            }                
            return Json(true);
        }
    }
}
