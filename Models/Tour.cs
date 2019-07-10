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
        [Display(Name = "Tour ID")]
        [Required(ErrorMessage = "Tour ID is required")]
        [MaxLength(30, ErrorMessage = "Tour ID can't be more than 30 characters")]
        public string Id { get; set; }

        [Display(Name = "Tour Name")]
        [Required(ErrorMessage = "Tour name is required")]
        [MaxLength(100, ErrorMessage = "Tour name can't be more than 100 characters")]
        public string Name { get; set; }


        [Display(Name = "Tour Image")]
        [Required(ErrorMessage = "Tour Image is required")]
        [MaxLength(200, ErrorMessage = "Image path is too long")]
        public string Image { get; set; }

        [MaxLength(500, ErrorMessage = "Description can't be more than 500 characters")]
        public string Description { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format")]
        public DateTime FromDate { get; set; }

        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format")]
        public DateTime ToDate { get; set; }

        [Display(Name = "Adult fare")]
        [Required(ErrorMessage = "Adult fare is required")]
        public decimal AdultFare { get; set; }

        [Display(Name = "Kid fare")]
        public decimal KidFare { get; set; }

        [Display(Name = "Maximum guest")]
        [Required(ErrorMessage = "Maximum guest is required")]
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
