using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models
{
    public class Tour
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal AdultFare { get; set; }

        public decimal KidFare { get; set; }

        public int MaxGuest { get; set; }

        public string Transport { get; set; }
        
        public bool IsActive { get; set; }

        public int Duration { get; set; }

        public IEnumerable<Destination> Destinations { get; set; }

        public IEnumerable<AppUser> Guides { get; set; }

        public string DurationStr()
        {
            string result = "";
            decimal day;
            decimal night;
            decimal remain = Duration % 24;
            if (FromDate.Hour < 6 || FromDate.Hour > 18)
            {
                night = Math.Floor((decimal)Duration / 24);
                day = night;
                if (remain > 12)
                {
                    day++;
                    night++;
                }
                else if (remain > 0 && remain <= 12)
                {
                    night++;
                }
            }
            else
            {
                day = Math.Floor((decimal)Duration / 24);
                night = day;
                if (remain > 12)
                {
                    day++;
                    night++;
                }
                else if (remain > 0 && remain <= 12)
                {
                    day++;
                }
            }
            if (day > 0)
                result += day + (day > 1 ? " days" : " day");
            if (night > 0)
                result += string.IsNullOrEmpty(result) ? "" : ", " + night + (night > 1 ? " nights" : " night");
            return result;
        }

        [BindNever]
        public int TakenSlot { get; set; }

        [BindNever]
        public int LeftSlot => MaxGuest - TakenSlot;
    }
}
