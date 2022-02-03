using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceLiteEntity.Models;
using ECommerceLiteEntity.ViewModels;
using Mapster;

namespace ECommerceLiteBLL.Repository
{
    public class Repositories
    {

    }

    public class CategoryRepo : RepositoryBase<Category, int>
    {
        public List<ProductCountModel> GetBaseCategoriesProductCount()
        {
            List<ProductCountModel> list = new List<ProductCountModel>();
            dbContext = new ECommerceLiteDAL.MyContext();
            var categoryList = this.Queryable()
                .Where(x => x.BaseCategoryId == null).ToList();
           

            foreach (var item in categoryList)
            {
                int productCount = 0;
                #region Ana Kategori için
                //Eğer bizim müşterimiz ana kategorilere ürün eklemeye izin verdiyse burada oluşan bug'ı aşağıdaki satırla çözeriz.
                var baseCategoryProductList = from p in dbContext.Products
                                              where p.CategoryId == item.Id
                                              select p;
                productCount = baseCategoryProductList.ToList().Count;
                #endregion
                //sub categoryleri
                var subCategoryList = this.Queryable()
                .Where(x => x.BaseCategoryId == item.Id).ToList();

                foreach (var subitem in subCategoryList)
                {
                    var productList = from p in dbContext.Products
                                      where p.CategoryId == subitem.Id
                                      select p;
                    productCount += productList.ToList().Count;
                }
                list.Add(new ProductCountModel()
                {
                    BaseCategory = item.Adapt<CategoryViewModel>(),
                    BaseCategoryName = item.CategoryName,
                    ProductCount = productCount
                });
            }
            return list;
        }
    }
    public class ProductRepo : RepositoryBase<Product, int> { }
    public class OrderRepo : RepositoryBase<Order, int> { }
    public class OrderDetailRepo : RepositoryBase<OrderDetail, int> { }
    public class CustomerRepo : RepositoryBase<Customer, string> { }
    public class AdminRepo : RepositoryBase<Admin, string> { }
    public class PassiveUserRepo : RepositoryBase<PassiveUser, string> { }
    public class ProductPictureRepo : RepositoryBase<ProductPicture, int> { }
}
