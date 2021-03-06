﻿using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MIMap.Tools
{
    public class SessionCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            return;
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session != null && session["UserAccount"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                                { "Controller", "Account" },
                                { "Action", "Login" }
                                });
            }
        }
    }
}