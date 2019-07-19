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

namespace ZTourist.Controllers
{
    [Authorize(Policy = "Customer")]
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

        public async Task<IActionResult> Index(int page = 1)
        {
            OrderViewModel model = new OrderViewModel();
            model.Skip = (page - 1) * model.Fetch;
            AppUser user = await userManager.FindByNameAsync(User.Identity.Name);
            int total = await touristDAL.GetTotalOrdersByUserIdAsync(user.Id);
            PageInfo pageInfo = new PageInfo
            {
                TotalItems = total,
                ItemPerPage = model.Fetch,
                PageAction = nameof(Index),
                CurrentPage = page
            };
            model.Orders = await touristDAL.FindOrdersByUserIdAsync(user.Id, model.Skip, model.Fetch);
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
            return View(model);
        }

        [ImportModelState]
        [ExportModelState]
        public async Task<IActionResult> Checkout()
        {
            if (cart.Lines.Count() <= 0)
            {
                ModelState.AddModelError("", "There is no items in cart to check out");
            }
            else
            {
                foreach (CartLine cartLine in cart.Lines) //load and check tours in cart is valid
                {
                    Tour tour = await touristDAL.FindTourByTourIdAsync(cartLine.Tour.Id); //find tour which is active
                    if (tour == null || tour.FromDate < DateTime.Now)
                    {
                        ModelState.AddModelError("", "Tour " + cartLine.Tour.Id.ToUpper() + " is not existed or available");
                    }
                    else
                    {
                        cartLine.Tour = tour;
                        cartLine.Tour.TakenSlot = await touristDAL.GetTakenSlotByTourIdAsync(cartLine.Tour.Id);
                        if ((cartLine.AdultTicket + cartLine.KidTicket) > (cartLine.Tour.MaxGuest - cartLine.Tour.TakenSlot))
                        {
                            ModelState.AddModelError("", "Not enough tickets of tour " + cartLine.Tour.Id.ToUpper());
                        }
                    }
                } //end load tour

                AppUser customer = await userManager.FindByNameAsync(User.Identity.Name); //get user infomation
                if (!string.IsNullOrEmpty(cart.Coupon.Code))
                {
                    CouponCode cp = await touristDAL.FindCouponByCodeAsync(cart.Coupon.Code);
                    if (cp == null)
                    {
                        ModelState.AddModelError("", "Coupon Code " + cart.Coupon.Code.ToUpper() + " is not existed or out of date");
                        cart.RemoveCoupon(); // remove coupon code from cart
                    }
                    else
                    {
                        cart.Coupon = cp;
                    }
                }

                if (ModelState.IsValid)
                {
                    DateTime now = DateTime.Now;
                    Order order = new Order
                    {
                        Id = User.Identity.Name.ToUpper() + now.Year.ToString("0000") + now.Month.ToString("00") + now.Day.ToString("00")
                        + now.Hour.ToString("00") + now.Minute.ToString("00") + now.Second.ToString("00") + now.Millisecond.ToString("000"),
                        Cart = cart,
                        Customer = customer
                    };
                    return View(order); //if everything is ok then dispay view for user to confirm payment and place order
                }
            }
            return RedirectToAction("Index", "Cart"); //if model state has error, return to cart view to display error
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Pay(Order order)
        {
            if (ModelState.IsValid)
            {
                if (cart.Lines.Count() <= 0) // if cart is empty
                {
                    ModelState.AddModelError("", "There is no item in cart to place order");
                }
                else // if cart has items
                {
                    foreach (CartLine cartLine in cart.Lines) //load and check tours in cart is valid
                    {
                        Tour tour = await touristDAL.FindTourByTourIdAsync(cartLine.Tour.Id); //find tour which is active
                        if (tour == null || tour.FromDate < DateTime.Now)
                        {
                            ModelState.AddModelError("", "Tour " + cartLine.Tour.Id.ToUpper() + " is not existed or available");
                        }
                        else
                        {
                            cartLine.Tour = tour;
                            cartLine.Tour.TakenSlot = await touristDAL.GetTakenSlotByTourIdAsync(cartLine.Tour.Id);
                            if ((cartLine.AdultTicket + cartLine.KidTicket) > (cartLine.Tour.MaxGuest - cartLine.Tour.TakenSlot))
                            {
                                ModelState.AddModelError("", "Not enough tickets of tour " + cartLine.Tour.Id.ToUpper());
                            }
                        }
                    } //end load tour

                    if (!string.IsNullOrEmpty(cart.Coupon.Code))
                    {
                        CouponCode cp = await touristDAL.FindCouponByCodeAsync(cart.Coupon.Code);
                        if (cp == null)
                        {
                            ModelState.AddModelError("", "Coupon Code " + cart.Coupon.Code.ToUpper() + " is not existed or out of date");
                            cart.RemoveCoupon(); // remove coupon code from cart

                        }
                        else
                        {
                            cart.Coupon = cp;
                        }
                    }
                } //end of check cart                

                if (!ModelState.IsValid)
                    return RedirectToAction("Index", "Cart"); // if tour items or coupon is invalid then redirect to cart view to display error

                // if everything is valid, then place order
                if (order.Comment == null)
                    order.Comment = "";
                order.Status = "Processing";
                order.OrderDate = DateTime.Now;
                order.Customer = await userManager.FindByNameAsync(User.Identity.Name);
                order.Cart = cart;
                if (!await touristDAL.AddOrderAsync(order))
                {
                    ModelState.AddModelError("", "Place order failed! Please recheck your items and infomation or try a moment later");
                }
                else // if insert successfully
                {
                    cart.Clear(); // refresh session cart
                    return RedirectToAction(nameof(Details), new { order.Id });
                }
            }
            return RedirectToAction(nameof(Checkout)); // if order input is invalid or insert is failed then display error at check out view
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
