using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models.ViewModels
{
    public class TourSearchViewModel
    {
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
                        break;
                }
            }
        }
    }

    public class TourViewModel
    {
        [Display(Name = "Tour ID")]
        [Required(ErrorMessage = "Tour ID is required")]
        [MaxLength(30, ErrorMessage = "Tour ID can't be more than 30 characters")]
        public string Id { get; set; }

        [Display(Name = "Tour Title")]
        [Required(ErrorMessage = "Tour title is required")]
        [MaxLength(100, ErrorMessage = "Tour title can't be more than 100 characters")]
        public string Name { get; set; }

        [Display(Name = "Tour Image")]
        [MaxLength(200, ErrorMessage = "Image path is too long")]
        public string Image { get; set; }

        [MaxLength(500, ErrorMessage = "Description can't be more than 500 characters")]
        public string Description { get; set; } = "";

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
        [Range(0.0000001, (Double) Decimal.MaxValue, ErrorMessage = "Adult fare must be greater than zero")]
        public decimal AdultFare { get; set; } = 0;

        [Display(Name = "Kid fare")]
        [Required(ErrorMessage = "Kid fare is required")]
        [Range(0, (Double)Decimal.MaxValue, ErrorMessage = "Kid fare must be greater or equals to zero")]
        public decimal KidFare { get; set; } = 0;

        [Display(Name = "Maximum guest")]
        [Required(ErrorMessage = "Maximum guest is required")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Maximum guest must be greater than zero")]
        public int MaxGuest { get; set; }

        [MaxLength(50, ErrorMessage = "Transport can't be more than 50 characters")]
        public string Transport { get; set; } = "";
        
        public bool IsActive { get; set; }

        public int Duration { get; set; }

        [Required(ErrorMessage = "Please select departure and destinations")]
        public IList<string> Destinations { get; set; }

        [Required(ErrorMessage = "Please select at least a guide")]
        public IList<string> Guides { get; set; }

        public IFormFile Photo { get; set; }

        public SelectList DestinationItems { get; set; }

        public SelectList GuideItems { get; set; }
    }
}
