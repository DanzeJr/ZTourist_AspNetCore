using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(login.UserName);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, login.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(login.ReturnUrl ?? "/");
                    }
                }
                ModelState.AddModelError("", "Invalid User or Password");
            }
            LoginSignUpModel model = new LoginSignUpModel
            {
                LoginModel = login
            };
            return View(model);
        }

        public async Task<IActionResult> SignUp(string username, string email, string returnUrl)
        {
            bool isExistedUser = false;
            bool isExistedEmail = false;
            if (username != null)
            {
                isExistedUser = await userManager.FindByNameAsync(username) == null ? false : true;
            }
            if (email != null)
            {
                isExistedEmail = await userManager.FindByEmailAsync(email) == null ? false : true;
            }

            if (isExistedUser)
            {
                ModelState.AddModelError("", $"User name '{username}' is already taken");
            }
            if (isExistedEmail)
            {
                ModelState.AddModelError("", $"Email '{email}' is already taken");
            }

            LoginSignUpModel model = new LoginSignUpModel
            {
                SignUpModel = new SignUpModel { UserName = username, Email = email }
            };
            return View("Login", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpModel signUp)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = signUp.UserName,
                    Email = signUp.Email,
                    FirstName = signUp.FirstName,
                    LastName = signUp.LastName,
                    Gender = signUp.Gender,
                    Address = signUp.Address,
                    BirthDate = signUp.BirthDate,
                    PhoneNumber = signUp.Tel,
                    Avatar = "images/avatars/ZAvatar.png",
                    RegisterDate = DateTime.Now
                };

                IdentityResult result = await userManager.CreateAsync(user, signUp.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            LoginSignUpModel model = new LoginSignUpModel
            {
                SignUpModel = signUp
            };
            return View("Login", model);
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
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> IsExistedUsername(string username)
        {
            if (username != null)
                if (await userManager.FindByNameAsync(username) != null)
                {
                    return Json($"User name '{username}' is already taken");
                }
            return Json(true);
        }

        public async Task<IActionResult> IsExistedEmail(string email)
        {
            if (email != null)
            if (await userManager.FindByEmailAsync(email) != null)
            {
                return Json($"Email '{email}' is already taken");
            }
            return Json(true);
        }
    }
}
