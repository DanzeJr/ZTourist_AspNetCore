using Microsoft.AspNetCore.Mvc;
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
        [StringLength(10, ErrorMessage = "Username can't be greater than 10 characters")]
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
        [StringLength(10, ErrorMessage = "Username can't be greater than 10 characters")]
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

        [Required(ErrorMessage = "Please accept Terms & Privacy")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept Terms & Privacy")]
        public bool AcceptTerm { get; set; }
    }
}
