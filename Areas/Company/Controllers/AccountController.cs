using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
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
        private readonly ILogger logger;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = new LoggerConfiguration()
                .WriteTo.AzureBlobStorage(configuration["Data:StorageAccount"], Serilog.Events.LogEventLevel.Information, $"logs", "{yyyy}/{MM}/{dd}/log.txt").CreateLogger();
        }

        [Authorize(Policy = "OnlyAnonymous")]
        [ImportModelState]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        
        [Authorize(Policy = "OnlyAnonymous")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Login(LoginModel login)
        {
            try
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
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }            
        }
        
        [Authorize(Policy = "OnlyAnonymous")]
        public IActionResult GoogleLogin(string returnUrl)
        {
            try
            {
                string redirectUrl = Url.Action("ExternalResponse", "Account", new { ReturnUrl = returnUrl });
                var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
                return new ChallengeResult("Google", properties);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }            
        }
        
        [Authorize(Policy = "OnlyAnonymous")]
        public IActionResult FaceBookLogin(string returnUrl)
        {
            try
            {
                string redirectUrl = Url.Action("ExternalResponse", "Account", new { ReturnUrl = returnUrl });
                var properties = signInManager.ConfigureExternalAuthenticationProperties("FaceBook", redirectUrl);
                return new ChallengeResult("FaceBook", properties);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }            
        }

        [Authorize(Policy = "OnlyAnonymous")]
        [ExportModelState]
        public async Task<IActionResult> ExternalResponse(string returnUrl = "/company")
        {
            try
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
                    if (await userManager.IsInRoleAsync(user, "Admin") || await userManager.IsInRoleAsync(user, "Guide")) // if user is admin or guide
                    {
                        if (await userManager.IsLockedOutAsync(user)) // if user is locked, then redirect to login and display message
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
                }
                // if user is not found or is not admin/guide, then redirect to login with invalid message
                ModelState.AddModelError("", "Invalid User or Password");
                return RedirectToAction(nameof(Login), new { returnUrl });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }            
        }

        [Authorize(Policy = "Employee")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await signInManager.SignOutAsync();
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }            
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> IsExistedUsername(string username)
        {
            try
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
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }            
        }

        public async Task<IActionResult> IsExistedEmail(string email, string username)
        {
            try
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
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }            
        }
    }
}
