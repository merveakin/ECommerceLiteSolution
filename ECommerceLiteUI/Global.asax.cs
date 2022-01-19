using ECommerceLiteBLL.Account;
using ECommerceLiteEntity.Enums;
using ECommerceLiteEntity.IdentityModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ECommerceLiteUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            var myRoleManager = MembershipTools.NewRoleManager();
            var theRoles = Enum.GetNames(typeof(TheIdentityRoles));
            foreach (var therole in theRoles)
            {
                if (!myRoleManager.RoleExists(therole))
                    myRoleManager.Create(new ApplicationRole()
                    {
                        Name = therole
                    });
            }
        }
    }
}
