using ECommerceLiteBLL.Account;
using ECommerceLiteEntity.IdentityModels;
using ECommerceLiteUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteBLL.Repository;

namespace ECommerceLiteUI.Controllers
{
    public class PartialsController : BaseController
    {
        //GLOBAL ZONE
        CategoryRepo myCategoryRepo = new CategoryRepo();
        ProductRepo myProductRepo = new ProductRepo();

        public PartialViewResult AdminSideBarResult()
        {
            //TODO : name surname alınacak
            TempData["NameSurname"] = "";
            return PartialView("_PartialAdminSideBar");
        }
        public PartialViewResult AdminSideBarMenuResult()
        {
            return PartialView("_PartialAdminSideBarMenu");
        }
        public PartialViewResult UserNameSurnameOnHomePage()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var loggedInUser = MembershipTools.GetUser();
                return PartialView("_PartialUserNameSurnameOnHomePage", loggedInUser);
            }
            return PartialView("_PartialUserNameSurnameOnHomePage", null);
        }
        public PartialViewResult ShoppingCart()
        {
            var shoppingCart = Session["ShoppingCart"] as List<CartViewModel>;

            if (shoppingCart == null)
            {
                return PartialView("_PartialShoppingCart", new List<CartViewModel>());
            }

            else
            {
                return PartialView("_PartialShoppingCart", shoppingCart);
            }
        }
        public PartialViewResult AdminSideBarCategories()
        {
            TempData["AllCategoriesCount"] =
                myCategoryRepo.Queryable()
                .Where(x => x.BaseCategory == null).ToList().Count;

            return PartialView("_PartialAdminSideBarCategories");
        }
        public PartialViewResult AdminSideBarProducts()
        {
            TempData["CategoryProductsCount"] = myProductRepo.GetAll().Count;
            return PartialView("_PartialAdminSideBarProducts");
        }
    }
}