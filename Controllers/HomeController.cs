using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZTourist.Controllers
{
    [Authorize(Policy = "NotEmployee")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                if (statusCode == 404)
                {
                    ViewBag.StatusCode = statusCode;
                    ViewBag.Title = "Page Not Found";
                    ViewBag.Message = "The link you followed may be broken, or the page may have been removed.";
                }
            }
            return View();
        }
    }
}
