using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models.ViewModels
{
    public class CartViewModel
    {
        public Cart Cart { get; set; }

        public CartLine CartLine { get; set; }

        public CouponCode Coupon { get; set; }
    }
}
