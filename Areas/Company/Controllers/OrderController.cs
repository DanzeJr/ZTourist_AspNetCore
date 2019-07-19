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
using ZTourist.Models.ViewModels;

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

        public async Task<IActionResult> Index(string status = "Processing", int page = 1)
        {
            OrderViewModel model = new OrderViewModel { Status = status };
            model.Skip = (page - 1) * model.Fetch;
            int total = await touristDAL.GetTotalOrdersByStatusAsync(model.Status);
            PageInfo pageInfo = new PageInfo
            {
                TotalItems = total,
                ItemPerPage = model.Fetch,
                PageAction = nameof(Index),
                CurrentPage = page
            };
            model.Orders = await touristDAL.GetOrdersByStatusAsync(model.Status, model.Skip, model.Fetch);
            if (model.Orders != null && model.Orders.Count() > 0)
            {
                foreach (Order order in model.Orders)
                {
                    order.Cart.Lines = await touristDAL.GetDetailsByOrderIdAsync(order.Id);
                }
            }
            model.PageInfo = pageInfo;
            if (model.Status.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
                ViewBag.Title = "Accepted Order";
            else if (model.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                ViewBag.Title = "Cancelled Order";
            else
                ViewBag.Title = "Pending Order";
            return View(model);
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
