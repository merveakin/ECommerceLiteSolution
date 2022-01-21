using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteEntity.Models;
using ECommerceLiteBLL.Repository;

namespace ECommerceLiteUI.Controllers
{
    public class ProductController : Controller
    {
        //GLOBAL ZONE
        ProductRepo myProductRepo = new ProductRepo();
        CategoryRepo myCategoryRepo = new CategoryRepo();


        public ActionResult ProductList()
        {
            var allProductList =
                myProductRepo.GetAll();
            return View(allProductList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            List<SelectListItem> allCategories = new List<SelectListItem>();
            myCategoryRepo.GetAll().ForEach(x => allCategories.Add(new SelectListItem()
            {
                Text = x.CategoryName,
                Value = x.Id.ToString()
            }));
            ViewBag.CategoryList = allCategories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Data entries must be correct!");
                    return View(model);
                }
                int insertResult = myProductRepo.Insert(model);
                if (insertResult > 0)
                {
                    return RedirectToAction("ProductList", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Error occurred while adding product! Try Again!");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unexpected Error!");
                //TODO : ex loglanacak..
                return View();
            }
        }
    }
}