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

        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            ViewBag.Title = "Login";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind(Prefix = nameof(LoginSignUpModel.LoginModel))] LoginModel login)
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
                        if (!string.IsNullOrEmpty(login.ReturnUrl) && Url.IsLocalUrl(login.ReturnUrl))
                            return Redirect(login.ReturnUrl);
                        else
                            return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid User or Password");
            }
            LoginSignUpModel model = new LoginSignUpModel
            {
                LoginModel = login
            };
            ViewBag.returnUrl = login.ReturnUrl;
            ViewBag.Title = "Login";
            return View(model);
        }

        public IActionResult SignUp(LoginSignUpModel model)
        {
            ViewBag.returnUrl = model.SignUpModel.ReturnUrl;
            ViewBag.Title = "Registration";
            return View("Login", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind(Prefix = nameof(LoginSignUpModel.SignUpModel))] SignUpModel signUp)
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
                    Avatar = "https://ztourist.blob.core.windows.net/others/avatar.png",
                    RegisterDate = DateTime.Now
                };

                IdentityResult result = await userManager.CreateAsync(user, signUp.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(signUp.ReturnUrl) && Url.IsLocalUrl(signUp.ReturnUrl))
                        return Redirect(signUp.ReturnUrl);
                    else
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
            ViewBag.returnUrl = signUp.ReturnUrl;
            ViewBag.Title = "Registration";
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

        public async Task<IActionResult> IsExistedEmail(string email)
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
                    if (User?.Identity?.Name != null)
                    {
                        user = await userManager.FindByNameAsync(User.Identity.Name);
                        if (email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)) // if email is requester's email
                            return Json(true);
                    }
                    return Json($"Email '{email}' is already taken");
                }
            }                
            return Json(true);
        }
    }
}
