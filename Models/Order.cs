using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models
{
    public class Order
    {
        public string Id { get; set; }

        public AppUser Customer { get; set; }

        public Cart Cart { get; set; }

        [MaxLength(500, ErrorMessage = "Comment can't be more than 500 characters")]
        public string Comment { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }

    }
}
