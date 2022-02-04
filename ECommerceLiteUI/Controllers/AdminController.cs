using ECommerceLiteBLL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceLiteUI.Controllers
{
    [Authorize (Roles="Admin")]
    public class AdminController : BaseController
    {
        //GLOBAL ZONE
        OrderRepo myOrderRepo = new OrderRepo();
        CategoryRepo myCategoryRepo = new CategoryRepo();
        // GET: Admin
        public ActionResult Dashboard()
        {
            var OrderList = myOrderRepo.GetAll();
            //1 aylık sipariş sayısı >>>
            var newOrderCount = OrderList.Where(x => x.RegisterDate >= DateTime.Now.AddMonths(-1)).ToList().Count;

            ViewBag.NewOrderCount = newOrderCount;

            return View();
        }
        
        public ActionResult Dashboard2()
        {
            var OrderList = myOrderRepo.GetAll();
            //1 aylık sipariş sayısı >>>
            var newOrderCount = OrderList.Where(x => x.RegisterDate >= DateTime.Now.AddMonths(-1)).ToList().Count;
            ViewBag.NewOrderCount = newOrderCount;

            var model = myCategoryRepo.GetBaseCategoriesProductCount();
            return View(model);
        }



        [AllowAnonymous]
        public ActionResult Deneme()
        {
            return View();
        }
    }
}