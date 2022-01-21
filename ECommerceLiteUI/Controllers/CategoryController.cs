using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteBLL.Repository;

namespace ECommerceLiteUI.Controllers
{
    public class CategoryController : Controller
    {
        //GLOBAL ZONE
        CategoryRepo myCategoryRepo = new CategoryRepo();

        // GET: Category
        public ActionResult CategoryList()
        {
            var allCategories = myCategoryRepo.Queryable().Where(x => x.BaseCategoryId == null).ToList();
            ViewBag.CategoryCount = allCategories.Count;
            return View(allCategories);
        }
    }
}