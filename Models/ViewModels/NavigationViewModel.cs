using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models.ViewModels
{
    public class NavigationViewModel
    {
        public IEnumerable<Tour> PopularTours { get; set; }

        public Cart Cart { get; set; }

        public string Avatar { get; set; }
    }
}
