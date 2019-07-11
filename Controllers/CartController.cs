using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZTourist.Infrastructure;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Controllers
{
    public class CartController : Controller
    {
        private readonly Cart cart;
        private readonly TouristDAL touristDAL;

        public CartController(Cart cart, TouristDAL touristDAL)
        {
            this.cart = cart;
            this.touristDAL = touristDAL;
        }

        [ImportModelState]
        public async Task<IActionResult> Index()
        {
            CouponCode cp = null;
            if (!string.IsNullOrEmpty(cart.CouponCode)) // check if cart contains coupon code
            {
                cp = await touristDAL.FindCouponByCodeAsync(cart.CouponCode);
                if (cp == null) // if coupon code is not found or outdated
                {
                    ModelState.AddModelError("", "Coupon Code is not existed or out of date");
                    cart.RemoveCoupon(); // remove coupon code from cart
                }
            }
            if (cart != null && cart.Lines.Count() > 0)
            {
                foreach (CartLine cartLine in cart.Lines)
                {
                    cartLine.Tour = await touristDAL.FindTourByTourIdAsync(cartLine.Tour.Id);
                }
            }
            CartViewModel model = new CartViewModel
            {
                Cart = cart,
                Coupon = cp ?? new CouponCode()
            };
            return View(model);
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> Add(CartLine model)
        {
            if (ModelState.IsValid)
            {
                if (!model.AdultTicket.HasValue)
                    model.AdultTicket = 0;
                if (!model.KidTicket.HasValue)
                    model.KidTicket = 0;
                if (model.AdultTicket == 0 && model.KidTicket == 0) // if number of tickets = 0
                {
                    ModelState.AddModelError("", "You must choose at least 1 ticket for adult or kid");
                }
                else
                {
                    bool isExistedTour = false;
                    if (!string.IsNullOrWhiteSpace(model.Tour.Id))
                        isExistedTour = await touristDAL.IsExistedTourIdAsync(model.Tour.Id);
                    if (!isExistedTour)
                    {
                        cart.RemoveItem(model.Tour.Id); // remove tour from cart if tour is not existed anymore
                        ModelState.AddModelError("", "Tour is not existed");
                    }
                    else
                    {
                        int takenSlot = await touristDAL.GetTakenSlotByTourIdAsync(model.Tour.Id);
                        int maxGuest = await touristDAL.GetMaxGuestByTourIdAsync(model.Tour.Id);
                        CartLine line = cart.Lines.FirstOrDefault(x => x.Tour.Id == model.Tour.Id);
                        int totalTicket = (line?.AdultTicket ?? 0) + (line?.KidTicket ?? 0);
                        if ((totalTicket + model.AdultTicket + model.KidTicket) > (maxGuest - takenSlot))
                        {
                            ModelState.AddModelError("", "Not enough tickets. You've already got " + totalTicket + " of this item");
                        }
                        else
                        {
                            cart.AddItem(model);
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }
            return RedirectToAction("Details", "Tour", new { model.Tour.Id });
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> Update([Bind(Prefix = nameof(CartViewModel.CartLine))] CartLine model)
        {
            if (ModelState.IsValid)
            {
                if (!model.AdultTicket.HasValue)
                    model.AdultTicket = 0;
                if (!model.KidTicket.HasValue)
                    model.KidTicket = 0;
                if (cart.Lines.Select(x => x.Tour.Id == model.Tour.Id).FirstOrDefault() == false) // if tour is not in cart
                {
                    ModelState.AddModelError("", "Tour is not in cart to update");                    
                }
                else if (model.AdultTicket == 0 && model.KidTicket == 0) // if number of tickets = 0
                {
                    ModelState.AddModelError("", "You must choose at least 1 ticket for adult or kid");
                }
                else
                {
                    bool isExistedTour = false;
                    if (!string.IsNullOrWhiteSpace(model.Tour.Id))
                        isExistedTour = await touristDAL.IsExistedTourIdAsync(model.Tour.Id);
                    if (!isExistedTour)
                    {
                        cart.RemoveItem(model.Tour.Id); // remove tour from cart if tour is not existed anymore
                        ModelState.AddModelError("", "Tour is not existed");
                    }
                    else
                    {
                        int takenSlot = await touristDAL.GetTakenSlotByTourIdAsync(model.Tour.Id);
                        int maxGuest = await touristDAL.GetMaxGuestByTourIdAsync(model.Tour.Id);
                        if ((model.AdultTicket + model.KidTicket) > (maxGuest - takenSlot))
                        {
                            ModelState.AddModelError("", "Not enough tickets");
                        }
                        else
                        {
                            cart.UpdateItem(model);                                
                        }
                    }
                }                
            }
            return RedirectToAction(nameof(Index));
        }

        [ExportModelState]
        public IActionResult Remove(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError("", "Please choose a tour to remove from cart");
            }
            else
            {
                if (cart.RemoveItem(id) <= 0)
                {
                    ModelState.AddModelError("", "Tour is not in cart to remove");
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            if (string.IsNullOrEmpty(couponCode))
            {
                ModelState.AddModelError("", "Please enter coupon code to continue");
            }
            else
            {
                CouponCode cp = await touristDAL.FindCouponByCodeAsync(couponCode);
                if (cp == null) // if coupon code is not found or outdated
                {
                    ModelState.AddModelError("", "Coupon Code is not existed or out of date");
                    // if there was old coupon code, leave it remain in cart
                }
                else
                {
                    cart.ApplyCoupon(couponCode);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
