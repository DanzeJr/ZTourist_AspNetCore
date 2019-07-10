using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models
{
    public class Destination
    {
        [Display(Name = "Destination ID")]
        [Required(ErrorMessage = "Destination ID is required")]
        [MaxLength(30, ErrorMessage = "Destination ID can't be more than 30 characters")]
        public string Id { get; set; }

        [Display(Name = "Destination Name")]
        [Required(ErrorMessage = "Destination name is required")]
        [MaxLength(100, ErrorMessage = "Destination name can't be more than 100 characters")]
        public string Name { get; set; }


        [Display(Name = "Destination Image")]
        [Required(ErrorMessage = "Destination Image is required")]
        [MaxLength(200, ErrorMessage = "Image path is too long")]
        public string Image { get; set; }

        [MaxLength(500, ErrorMessage = "Description can't be more than 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [MaxLength(50, ErrorMessage = "Country name is too long")]
        public string Country { get; set; }

        public bool IsActive { get; set; }
    }
}
