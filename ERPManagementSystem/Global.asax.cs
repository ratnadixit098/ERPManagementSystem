using ERPManagementSystem.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Threading;

namespace ERPManagementSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Timer timer = new Timer(CheckReminder, null,
               TimeSpan.Zero,
               TimeSpan.FromDays(1));
        }
      
        private void CheckReminder(object state)
        {
            ReminderController obj = new ReminderController();
            obj.SendFeeReminder();
        }
    }
}
