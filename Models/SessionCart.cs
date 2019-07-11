using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZTourist.Infrastructure;

namespace ZTourist.Models
{
    public class SessionCart : Cart
    {
        [JsonIgnore]
        public ISession Session { get; set; }

        public static Cart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();
            cart.Session = session;
            return cart;
        }
        
        public override void AddItem(CartLine line)
        {
            base.AddItem(line);
            Session.SetJson("Cart", this);
        }

        public override bool  UpdateItem(CartLine line)
        {
            bool result = base.UpdateItem(line);
            Session.SetJson("Cart", this);
            return result;
        }

        public override int RemoveItem(string id)
        {
            int result = base.RemoveItem(id);
            Session.SetJson("Cart", this);
            return result;
        }

        public override void ApplyCoupon(string code)
        {
            base.ApplyCoupon(code);
            Session.SetJson("Cart", this);
        }

        public override void RemoveCoupon()
        {
            base.RemoveCoupon();
            Session.SetJson("Cart", this);
        }

        public override void Clear()
        {
            base.Clear();
            Session.Remove("Cart");
        }
    }
}
