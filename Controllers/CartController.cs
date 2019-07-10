using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZTourist.Models;

namespace ZTourist.Controllers
{
    public class CartController : Controller
    {
        private readonly Cart cart;

        public CartController(Cart cart)
        {
            this.cart = cart;
        }

        public IActionResult Index()
        {
            return View(cart);
        }
    }
}
