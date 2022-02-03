//using ECommerceLiteEntity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.ViewModels
{
    public class CategoryViewModel /*: TheBase<int>*/
    {
        public int Id { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Register Date")]
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The length of the category name must be between 2 and 50 characters!")]
        public string CategoryName { get; set; }
        [StringLength(500, ErrorMessage = "The length of the category description must not exceed 500 characters!")]
        public string CategoryDescription { get; set; }
        public int? BaseCategoryId { get; set; }
    }
}
