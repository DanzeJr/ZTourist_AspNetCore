using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models
{
    public class Order
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public IEnumerable<Tour> Tours { get; set; }

        public int AdultTicket { get; set; }

        public int KidTicket { get; set; }

        public string CouponCode { get; set; }

        public string Comment { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }
    }
}
