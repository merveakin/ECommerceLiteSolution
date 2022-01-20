using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceLiteUI.Models
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Display(Name = "User Name")]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [StringLength(100)]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z]\w{4,14}$", ErrorMessage = @"	
The password's first character must be a letter, it must contain at least 5 characters and no more than 15 characters and no characters other than letters, numbers and the underscore may be used")]
        public string NewPassword { get; set; }
        [StringLength(100)]
        [Display(Name = "New Password Again")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}