using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    [Table("Categories")]
    public class Category : TheBase<int>
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The length of the category name must be between 2 and 50 characters!")]
        public string CategoryName { get; set; }
        [StringLength(500, ErrorMessage = "The length of the category description must not exceed 500 characters!")]
        public string CategoryDescription { get; set; }

        public int BaseCategoryId { get; set; }
        [ForeignKey("BaseCategoryId")]
        public virtual Category BaseCategory { get; set; }
        public virtual List<Product> ProductList { get; set; }
        public virtual List<Category> CategoryList { get; set; }
    }
}
