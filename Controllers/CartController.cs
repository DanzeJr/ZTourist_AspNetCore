using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZTourist.Infrastructure;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Controllers
{
    [Authorize(Policy = "NotEmployee")]
    public class CartController : Controller
    {
        private readonly Cart cart;
        private readonly TourDAL tourDAL;
        private readonly CouponDAL couponDAL;

        public CartController(Cart cart, TourDAL tourDAL, CouponDAL couponDAL)
        {
            this.cart = cart;
            this.tourDAL = tourDAL;
            this.couponDAL = couponDAL;
        }

        [ImportModelState]
        public async Task<IActionResult> Index()
        {
            CouponCode cp = null;
            if (!string.IsNullOrEmpty(cart.Coupon?.Code)) // check if cart contains coupon code
            {
                cp = await couponDAL.FindCouponByCodeAsync(cart.Coupon.Code);
                if (cp == null) // if coupon code is not found or outdated
                {
                    ModelState.AddModelError("", "Coupon Code " + cart.Coupon.Code.ToUpper() + " is not existed or out of date");
                    cart.RemoveCoupon(); // remove coupon code from cart
                }
                else
                {
                    cart.Coupon = cp;
                }
            }
            if (cart.Lines.Count() > 0)
            {
                foreach (CartLine cartLine in cart.Lines)
                {
                    Tour tour = await tourDAL.FindTourByTourIdAsync(cartLine.Tour.Id); //find tour which is active
                    if (tour == null || tour.FromDate < DateTime.Now)
                    {
                        ModelState.AddModelError("", "Tour " + cartLine.Tour.Id.ToUpper() + " is not existed or available");
                    } else
                    {
                        cartLine.Tour = tour;
                        cartLine.Tour.TakenSlot = await tourDAL.GetTakenSlotByTourIdAsync(cartLine.Tour.Id);
                        if ((cartLine.AdultTicket + cartLine.KidTicket) > (cartLine.Tour.MaxGuest - cartLine.Tour.TakenSlot))
                        {
                            ModelState.AddModelError("", "Not enough tickets of tour " + cartLine.Tour.Id.ToUpper());
                        }
                    }
                }
            }
            CartViewModel model = new CartViewModel
            {
                Cart = cart
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
                    bool isAvailableTour = false;
                    if (!string.IsNullOrWhiteSpace(model.Tour.Id))
                        isAvailableTour = await tourDAL.IsAvailableTourAsync(model.Tour.Id);
                    if (!isAvailableTour)
                    {
                        ModelState.AddModelError("", "Tour " + model.Tour.Id.ToUpper() + " is not existed or available");
                        return RedirectToAction(nameof(Index)); //if tour is not available then report error at cart view since tour details can be found
                    }
                    else
                    {
                        int takenSlot = await tourDAL.GetTakenSlotByTourIdAsync(model.Tour.Id);
                        int maxGuest = await tourDAL.GetMaxGuestByTourIdAsync(model.Tour.Id);
                        CartLine line = cart.Lines.FirstOrDefault(x => x.Tour.Id == model.Tour.Id); // find if tour is in cart
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
            return RedirectToAction("Details", "Tour", new { model.Tour.Id }); //report error at tour details if model properties is invalid or not enough tickets
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
                    bool isAvailableTour = false;
                    if (!string.IsNullOrWhiteSpace(model.Tour.Id))
                        isAvailableTour = await tourDAL.IsAvailableTourAsync(model.Tour.Id);
                    if (!isAvailableTour)
                    {
                        ModelState.AddModelError("", "Tour " + model.Tour.Id.ToUpper() + " is not existed or available");
                    }
                    else
                    {
                        int takenSlot = await tourDAL.GetTakenSlotByTourIdAsync(model.Tour.Id);
                        int maxGuest = await tourDAL.GetMaxGuestByTourIdAsync(model.Tour.Id);
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
                CouponCode cp = await couponDAL.FindCouponByCodeAsync(couponCode);
                if (cp == null) // if coupon code is not found or outdated
                {
                    ModelState.AddModelError("", "Coupon Code " + couponCode.ToUpper() + " is not existed or out of date");
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
