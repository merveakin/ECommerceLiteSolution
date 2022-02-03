using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteBLL.Repository;

namespace ECommerceLiteUI.Controllers
{
    public class ChartController : Controller
    {
        //GLOBAL ZONE
        CategoryRepo myCategoryRepo = new CategoryRepo();


        public ActionResult VisualizePieChartResult()
        {
            //PieChartta göstermek istediğimiz datayı alacağız
            //Bu detayı Dashboard'daki ajax işlemine gönderebilmek için Return JSon ile işlem yapacağız.
            var data = myCategoryRepo.GetBaseCategoriesProductCount();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}