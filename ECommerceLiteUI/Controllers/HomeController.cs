using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteBLL.Repository;
using Mapster;
using ECommerceLiteUI.Models;

namespace ECommerceLiteUI.Controllers
{
    public class HomeController : Controller
    {
        //GLOBAL ZONE
        CategoryRepo myCategoryRepo = new CategoryRepo();
        ProductRepo myProductRepo = new ProductRepo();


        public ActionResult Index()
        {
            var categoryList = myCategoryRepo.Queryable().Where(x => x.BaseCategoryId == null).Take(4).ToList();
            ViewBag.CategoryList = categoryList;
            var productList = myProductRepo.GetAll();
            List<ProductViewModel> model = new List<ProductViewModel>();
            foreach (var item in productList)
            {
                model.Add(item.Adapt<ProductViewModel>());
            }
            foreach (var item in model)
            {
                item.SetCategory();
                item.SetProductPictures();
            }



            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AddToCart(int id)
        {
            try
            {
                var shoppingCart =
                    Session["ShoppingCart"] as List<CartViewModel>;
                if (shoppingCart==null)
                {
                    shoppingCart = new List<CartViewModel>();
                }
                if (id > 0)
                {
                    var product = myProductRepo.GetById(id);
                    if (product == null)
                    {
                        TempData["AddToCart"] = "Product addition failed. Try Again!";
                        return RedirectToAction("Index", "Home");
                    }

                    var productAddtoCart = product.Adapt <CartViewModel> ();
                    if (shoppingCart.Count(x => x.Id == productAddtoCart.Id) > 0)
                    {
                        shoppingCart.FirstOrDefault(x => x.Id == productAddtoCart.Id).Quantity++;
                    }

                    else
                    {
                        productAddtoCart.Quantity = 1;
                        shoppingCart.Add(productAddtoCart);
                    }

                    Session["ShoppingCart"] = shoppingCart;
                    TempData["AddToCart"]= "Product added";
                }

                return RedirectToAction("Index","Home");
            }
            catch (Exception ex)
            {
                TempData["AddToCart"] = "Product addition failed. Try Again!";
                return RedirectToAction("Index", "Home");

            }
        }


    }
}