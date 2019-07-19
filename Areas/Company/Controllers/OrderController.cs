using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZTourist.Infrastructure;
using ZTourist.Models;

namespace ZTourist.Areas.Company.Controllers
{
    [Area("Company")]
    [Authorize(Policy = "Admin")]
    public class OrderController : Controller
    {
        private readonly Cart cart;
        private readonly UserManager<AppUser> userManager;
        private readonly TouristDAL touristDAL;

        public OrderController(Cart cart, UserManager<AppUser> userManager, TouristDAL touristDAL)
        {
            this.cart = cart;
            this.userManager = userManager;
            this.touristDAL = touristDAL;
        }

        public IActionResult Index(string status, int page = 1)
        {

            return View();
        }



        public IActionResult Result()
        {
            Order order = TempData.Get<Order>("Order");
            if (order == null)
            {
                return NotFound(); // if tempdate does not contain order, which means this request is not redirected from Pay action, so response with 404 not found
            }
            //if contain order
            return View(order);
        }
    }
}
