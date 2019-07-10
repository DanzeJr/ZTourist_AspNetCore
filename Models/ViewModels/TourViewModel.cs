using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models.ViewModels
{
    public class TourViewModel
    {
        public string Title { get; set; }

        public IEnumerable<Tour> Tours { get; set; }

        public PageInfo PageInfo { get; set; }

        public List<SelectListItem> DurationItems { get; set; }

        public List<SelectListItem> PriceItems { get; set; }

        public SelectList DestinationItems { get; set; }

        public int Skip { get; set; } = 0;

        public int Fetch { get; set; } = 5;

        public string Id { get; set; }

        public string Name { get; set; }

        public string Destination { get; set; }

        public DateTime? FromDate { get; set; }

        public int? Duration { get; set; }

        public int? PriceRange { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public bool? IsActive { get; set; }

        public void InitSearchValues()
        {
            //set min max price
            if (PriceRange == null || PriceRange <= 0)
            {
                MinPrice = null;
                MaxPrice = null;
                PriceRange = null;
            }
            else
            {
                switch (PriceRange)
                {
                    case 1:
                        MinPrice = null;
                        MaxPrice = 50;
                        break;
                    case 2:
                        MinPrice = 50;
                        MaxPrice = 250;
                        break;
                    case 3:
                        MinPrice = 250;
                        MaxPrice = 500;
                        break;
                    case 4:
                        MinPrice = 500;
                        MaxPrice = 1000;
                        break;
                    case 5:
                        MinPrice = 1000;
                        MaxPrice = 1500;
                        break;
                    case 6:
                        MinPrice = 1500;
                        MaxPrice = 2000;
                        break;
                    case 7:
                        MinPrice = 2000;
                        MaxPrice = 2500;
                        break;
                    case 8:
                        MinPrice = 2500;
                        MaxPrice = null;
                        break;
                    default:
                        MinPrice = null;
                        MaxPrice = null;
                        PriceRange = null;
                        break;
                }
            }

            //set duration
            if (Duration != null)
            {
                if (Duration >= 8 || Duration <= 0)
                    Duration = null;
                else
                    Duration = Duration * 24;
            }
        }
    }
}
