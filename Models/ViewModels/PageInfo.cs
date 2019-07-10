using System;

namespace ZTourist.Models.ViewModels
{
    public class PageInfo
    {
        public int TotalItems { get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; } = 1;
        public string PageAction { get; set; }

        public int TotalPages => (int) Math.Ceiling((decimal) TotalItems / ItemPerPage);
    }
}
