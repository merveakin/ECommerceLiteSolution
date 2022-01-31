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
       [StringLength(8,ErrorMessage = "The length of the product code must be no more than 8 characters!")]
       [Index(IsUnique =true)]  //Ürün kodu her bir ürün için özeldir. Ve her ürünün kodu birbirinden farklı olmalıdır.
        public string ProductCode { get; set; }


        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
       
        [Required]
        public int Quantity { get; set; }
    
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public virtual List<ProductPicture> ProductPictureList { get; set; }
    }
}
