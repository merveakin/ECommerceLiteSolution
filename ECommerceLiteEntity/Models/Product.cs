using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    public class Product : TheBase<int>
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The length of the product name must be between 2 and 50 characters!")]
        public string ProductName { get; set; }
       
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Product description length must be no more than 500 characters!")]
        public string Description { get; set; }
       
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
       
        [Required]
        public int Quantity { get; set; }
    
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

    }
}
