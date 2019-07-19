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
                    if (!string.IsNullOrEmpty(order.Cart.Coupon.Code))
                        order.Cart.Coupon = await touristDAL.FindCouponByCodeAsync(order.Cart.Coupon.Code, order.OrderDate);
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

        [ImportModelState]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            Order order = await touristDAL.FindOrderByIdAsync(id);
            if (order == null) // if order is not existed
            {
                return NotFound();
            }
            order.Customer = await userManager.FindByIdAsync(order.Customer.Id);
            order.Cart.Lines = await touristDAL.GetDetailsByOrderIdAsync(id);
            if (!string.IsNullOrWhiteSpace(order.Cart.Coupon.Code))
                order.Cart.Coupon = await touristDAL.FindCouponByCodeAsync(order.Cart.Coupon.Code, order.OrderDate);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Accept(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            Order order = await touristDAL.FindOrderByIdAsync(id);
            if (order == null) // if order is not existed
            {
                return NotFound();
            }
            else if (order.Status != "Processing")
            {
                ModelState.AddModelError("", "Not allowed to update order status after accept or cancel");
            }
            else
            {
                if (await touristDAL.UpdateOrderStatusById(id, "Accepted"))
                    ModelState.AddModelError("", "Error occurs when accept order. Please try later");
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Cancel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            Order order = await touristDAL.FindOrderByIdAsync(id);
            if (order == null) // if order is not existed
            {
                return NotFound();
            }
            else if (order.Status != "Processing")
            {
                ModelState.AddModelError("", "Not allowed to update order status after accept or cancel");
            }
            else
            {
                if (await touristDAL.UpdateOrderStatusById(id, "Cancelled"))
                    ModelState.AddModelError("", "Error occurs when cancel order. Please try later");
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
