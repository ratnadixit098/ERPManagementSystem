using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPManagementSystem.Models
{
    public class CheckSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = HttpContext.Current.Session["UserName"];

            if (session == null)
            {
                filterContext.Result = new RedirectResult("/Home/index");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}