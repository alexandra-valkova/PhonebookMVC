﻿using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PhonebookMVC.ViewModels.Users
{
    public class UsersCreateEditVM : BaseVM
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 2)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Repeat Password")]
        [Compare("Password")]
        public string RepeatPassword { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Administrator")]
        public bool IsAdmin { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }

        [Display(Name = "Profile Picture")]
        public string ImageName { get; set; }
    }
}