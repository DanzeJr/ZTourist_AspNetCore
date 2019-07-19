using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZTourist.Infrastructure;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Controllers
{
    [Authorize(Policy = "NotEmployee")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        [Authorize(Policy = "OnlyAnonymous")]
        [ImportModelState]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            ViewBag.Title = "Login";
            return View();
        }


        [Authorize(Policy = "OnlyAnonymous")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Login([Bind(Prefix = nameof(LoginSignUpModel.LoginModel))] LoginModel login)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(login.UserName);
                if (user != null && await userManager.IsInRoleAsync(user, "Customer"))
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
                            return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid User or Password");
            }
            return RedirectToAction(nameof(Login), new { returnUrl = login.ReturnUrl });
        }

        [Authorize(Policy = "OnlyAnonymous")]
        [ImportModelState]
        public IActionResult SignUp(LoginSignUpModel model)
        {
            ViewBag.returnUrl = model?.SignUpModel?.ReturnUrl ?? HttpContext.Request.Query["returnUrl"];
            ViewBag.email = HttpContext.Request.Query["email"];
            ViewBag.Title = "Registration";
            return View("Login", model);
        }

        [Authorize(Policy = "OnlyAnonymous")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
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
                    result = await userManager.AddToRoleAsync(user, "Customer"); // assign customer role to user
                    if (result.Succeeded) // if role is assigned to user
                    {
                        string returnUrl;
                        if (!string.IsNullOrEmpty(signUp.ReturnUrl) && Url.IsLocalUrl(signUp.ReturnUrl))
                            returnUrl = signUp.ReturnUrl;
                        else
                            returnUrl = Url.Action("", "Home");
                        return RedirectToAction(nameof(Login), new { returnUrl });
                    }
                    else
                    {
                        IdentityResult r = await userManager.DeleteAsync(user); // if role is not assigned to user then delete it
                        foreach (IdentityError error in r.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return RedirectToAction(nameof(SignUp), new { SignUpModel = new SignUpModel { ReturnUrl = signUp.ReturnUrl } });
        }

        [Authorize(Policy = "OnlyAnonymous")]
        public IActionResult GoogleLogin(string returnUrl)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Account", new { returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        [Authorize(Policy = "OnlyAnonymous")]
        public IActionResult FacebookLogin(string returnUrl)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Account", new { returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return new ChallengeResult("Facebook", properties);
        }

        [Authorize(Policy = "OnlyAnonymous")]
        [ExportModelState]
        public async Task<IActionResult> ExternalResponse(string returnUrl = "/")
        {
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null) // if external login info is not available then redirect to login
            {
                return RedirectToAction(nameof(Login));
            }
            // if external login info is available
            string email = info.Principal.FindFirst(ClaimTypes.Email).Value;
            AppUser user = await userManager.FindByEmailAsync(email);
            if (user != null) // if there is a user with this email
            {
                if (!await userManager.IsInRoleAsync(user, "Customer")) // if user is not customer then redirect to login with invalid message
                {
                    ModelState.AddModelError("", "Invalid User or Password");
                    return RedirectToAction(nameof(Login), new { returnUrl });
                }
                else if (await userManager.IsLockedOutAsync(user)) // if user is locked customer, then redirect to login and display message
                {
                    ModelState.AddModelError("", "Your account has been locked");
                    return RedirectToAction(nameof(Login), new { returnUrl });
                }
                var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
                if (result.Succeeded) // if login successfully
                {
                    return Redirect(returnUrl);
                }
                else // if login with external login failed
                {
                    IdentityResult identityResult = await userManager.AddLoginAsync(user, info); // add external login to that user
                    if (identityResult.Succeeded)
                    {
                        await signInManager.SignInAsync(user, false);
                        return Redirect(returnUrl);
                    }
                    else // if add external login failed then redirect to login with message
                    {
                        ModelState.AddModelError("", "Error occurs! Try again later");
                    }
                    return RedirectToAction(nameof(Login), new { returnUrl });
                }
            }
            // if email is not belong to any user, then redirect to sign up
            return RedirectToAction(nameof(SignUp), new { email, returnUrl });
        }

        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
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
