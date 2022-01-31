using ECommerceLiteBLL.Repository;
using ECommerceLiteEntity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceLiteUI.Models
{
    public class ProductViewModel
    {
        CategoryRepo myCategoryRepo = new CategoryRepo();
        ProductPictureRepo myProductPictureRepo = new ProductPictureRepo();


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

        private Category _category = new Category();
        public Category CategoryOfProduct { get; set; }


        public List<ProductPicture> PicturesOfProduct { get; set; }

        public List<string> ProductPicturesList { get; set; } = new List<string>();

        public List<HttpPostedFileBase> Files { get; set; } = new List<HttpPostedFileBase>();

        public void SetCategory()
        {
            if (CategoryId > 0)
            {
                CategoryOfProduct = myCategoryRepo.GetById(CategoryId);
                if (CategoryOfProduct.BaseCategoryId > 0)
                {
                    CategoryOfProduct.CategoryList = new List<Category>();

                    CategoryOfProduct.BaseCategory = myCategoryRepo.GetById(CategoryOfProduct.BaseCategoryId.Value);
                    CategoryOfProduct.CategoryList.Add(CategoryOfProduct.BaseCategory);
                    bool isOver = false;
                    Category theBaseCategory = CategoryOfProduct.BaseCategory;
                    while (!isOver)
                    {

                        if (theBaseCategory.BaseCategoryId > 0)
                        {
                            CategoryOfProduct.CategoryList.Add(myCategoryRepo.GetById(theBaseCategory.BaseCategoryId.Value));
                            theBaseCategory = myCategoryRepo.GetById(theBaseCategory.BaseCategoryId.Value);
                        }
                        else
                        {
                            isOver = true;
                        }
                    }
                    CategoryOfProduct.CategoryList = CategoryOfProduct.CategoryList.OrderBy(x => x.Id).ToList();
                }
            }
        }

        public void SetProductPictures()
        {
            if (Id > 0)
            {
                PicturesOfProduct = myProductPictureRepo.Queryable().Where(x => x.ProductId == Id).ToList();
            }
        }

    }

}