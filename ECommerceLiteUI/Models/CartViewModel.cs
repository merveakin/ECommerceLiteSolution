using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceLiteUI.Models
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The length of the product name must be between 2 and 50 characters!")]
        public string ProductName { get; set; }
        [StringLength(500, ErrorMessage = "The length of the product description must be 500 characters maximum!")]
        public string Description { get; set; }

        [StringLength(8, ErrorMessage = "The length of the product code must be no more than 8 characters!")]
        public string ProductCode { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int CategoryId { get; set; }

    }
}