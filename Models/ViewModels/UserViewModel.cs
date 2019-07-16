using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace ZTourist.Models.ViewModels
{
    public class LoginSignUpModel
    {
        public LoginModel LoginModel { get; set; }

        public SignUpModel SignUpModel { get; set; }
    }

    public class LoginModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username field is required")]
        [StringLength(10, ErrorMessage = "Username must be between 4 and 10 characters", MinimumLength = 4)]
        [RegularExpression("^(?=.{4,10}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$", ErrorMessage = "Username is invalid format")]
        public string UserName { get; set; }

        [UIHint("Password")]
        [Required(ErrorMessage = "Password field is required")]
        [MinLength(6, ErrorMessage = "Password must be greater than 6 characters")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class SignUpModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username field is required")]
        [StringLength(10, ErrorMessage = "Username must be between 4 and 10 characters", MinimumLength = 4)]
        [RegularExpression("^(?=.{4,10}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$", ErrorMessage = "Username is invalid format")]
        [Remote(action: "IsExistedUsername", controller: "Account")]
        public string UserName { get; set; }

        [UIHint("Password")]
        [Required(ErrorMessage = "Password field is required")]
        [MinLength(6, ErrorMessage = "Password must be greater than 6 characters")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [UIHint("Password")]
        [Required(ErrorMessage = "Confirm Password field is required")]
        [MinLength(6, ErrorMessage = "Confirm Password must be greater than 6 characters")]
        [Compare(nameof(Password), ErrorMessage = "Password mismatch")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please select your gender")]
        public string Gender { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }

        [Display(Name = "Birth Date")]
        [UIHint("Text")]
        [Required(ErrorMessage = "Please enter your birth date")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Please enter your address")]
        public string Address { get; set; }

        [UIHint("Email")]
        [Required(ErrorMessage = "Email field is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        [Remote(action: "IsExistedEmail", controller: "Account")]
        public string Email { get; set; }

        [Display(Name = "Phone number")]
        [UIHint("Tel")]
        [Required(ErrorMessage = "Phone number field is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Tel { get; set; }
        
        public string ReturnUrl { get; set; }
    }

    public class ProfileModel
    {
        [Required(ErrorMessage = "Please select your gender")]
        public string Gender { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }

        [Display(Name = "Birth Date")]
        [UIHint("Text")]
        [Required(ErrorMessage = "Please enter your birth date")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Please enter your address")]
        public string Address { get; set; }

        [UIHint("Email")]
        [Required(ErrorMessage = "Email field is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        [Remote(action: "IsExistedEmail", controller: "Account")]
        public string Email { get; set; }

        [Display(Name = "Phone number")]
        [UIHint("Tel")]
        [Required(ErrorMessage = "Phone number field is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Tel { get; set; }

        public IFormFile Photo { get; set; }

        public string Avatar { get; set; }
    }

    public class PasswordModel
    {
        [Display(Name = "Current Password")]
        [UIHint("Password")]
        [Required(ErrorMessage = "Current Password is required")]
        [MinLength(6, ErrorMessage = "Password must be greater than 6 characters")]
        public string CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        [UIHint("Password")]
        [Required(ErrorMessage = "New Password is required")]
        [MinLength(6, ErrorMessage = "Password must be greater than 6 characters")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [UIHint("Password")]
        [Required(ErrorMessage = "Confirm New Password is required")]
        [MinLength(6, ErrorMessage = "Password must be greater than 6 characters")]
        [Compare(nameof(NewPassword), ErrorMessage = "Password mismatch")]
        public string ConfirmNewPassword { get; set; }
    }
}
