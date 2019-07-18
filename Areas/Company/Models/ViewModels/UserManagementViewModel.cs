using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Areas.Company.Models.ViewModels
{
    public class UserSearchViewModel
    {
        public List<UserViewModel> Users { get; set; }

        public PageInfo PageInfo { get; set; }

        public int Skip { get; set; } = 0;

        public int Fetch { get; set; } = 6;

        public bool? IsLocked { get; set; }
    }

    public class UserViewModel
    {
        public AppUser Profile { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }

    public class UserEditModel : ProfileModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please select at least a role")]
        public IEnumerable<string> Roles { get; set; }

        [Required(ErrorMessage = "Please choose status of this user")]
        public bool IsLocked { get; set; }

        public List<SelectListItem> RoleItems { get; set; }
    }
}
