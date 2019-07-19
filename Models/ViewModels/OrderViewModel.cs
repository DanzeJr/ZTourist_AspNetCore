using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models.ViewModels
{
    public class OrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; }

        public int Skip { get; set; } = 0;

        public int Fetch { get; set; } = 10;

        public string Status { get; set; }

        public PageInfo PageInfo { get; set; }
    }
}
