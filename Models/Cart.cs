using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models
{
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();

        public virtual List<CartLine> Lines
        {
            get { return lineCollection; }
            set { lineCollection = value; }
        }

        public CouponCode Coupon { get; set; } = new CouponCode();

        public virtual void AddItem(CartLine line)
        {
            CartLine temp = lineCollection
                .Where(x => x.Tour.Id == line.Tour.Id)
                .FirstOrDefault();

            if (temp == null)
            {
                lineCollection.Add(line);
            }
            else
            {
                temp.AdultTicket += line.AdultTicket;
                temp.KidTicket += line.KidTicket;
            }
        }

        public virtual bool UpdateItem(CartLine line)
        {
            CartLine temp = lineCollection
                .Where(x => x.Tour.Id == line.Tour.Id)
                .FirstOrDefault();

            if (temp != null)
            {
                temp.AdultTicket = line.AdultTicket;
                temp.KidTicket = line.KidTicket;
                return true;
            }
            return false;
        }

        public virtual int RemoveItem(string id) =>
            lineCollection.RemoveAll(x => x.Tour.Id == id);

        public virtual void ApplyCoupon(string code)
        {
            Coupon.Code = code;
        }

        public virtual void RemoveCoupon()
        {
            Coupon = new CouponCode();
        }

        public virtual decimal ComputeTotalValue() =>
            lineCollection.Sum(x => x.Tour.AdultFare * (x.AdultTicket ?? 0) + x.Tour.KidFare * (x.KidTicket ?? 0));

        public virtual void Clear()
        {
            lineCollection.Clear();
            Coupon = new CouponCode();
        }               

    }

    public class CartLine
    {
        public Tour Tour { get; set; }

        //[Required(ErrorMessage = "Please choose number of adult tickets")]
        [Range(0, Double.PositiveInfinity, ErrorMessage = "Number of adult tickets must be at least 0")]
        public int? AdultTicket { get; set; } = 0;

        [Range(0, Double.PositiveInfinity, ErrorMessage = "Number of kid tickets must be at least 0")]
        public int? KidTicket { get; set; } = 0;

        public decimal SubTotal()
        {
            if (Tour == null)
            {
                return 0;
            }
            return Tour.AdultFare * (AdultTicket ?? 0) + Tour.KidFare * (KidTicket ?? 0);
        }
    }

}
