using ECommerceLiteEntity.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceLiteEntity.IdentityModels
{
    public class ApplicationUser: IdentityUser
    {
        [StringLength(maximumLength:25,MinimumLength =2,ErrorMessage = "Your name must be between 2 and 25 characters long!")]
        [Display(Name="First Name")]
        [Required]
        public string Name { get; set; }

        [StringLength(maximumLength: 25, MinimumLength = 2, ErrorMessage = "Your surname must be between 2 and 25 characters long!")]
        [Display(Name = "Last Name")]
        [Required]
        public string Surname { get; set; }

        [Display(Name = "Register Date")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        //TODO : Guid'in kaç haneli olduğuna bakıp StringLength attribute tanımlayacağız.
        public string ActivationCode { get; set; }
        public virtual List<Customer> CustomerList { get; set; }
        public virtual List<Admin> AdminList { get; set; }
        public virtual List<PassiveUser> PassiveUserList { get; set; }
    }
}
