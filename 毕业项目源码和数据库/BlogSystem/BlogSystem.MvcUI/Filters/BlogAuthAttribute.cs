using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BlogSystem.MvcUI.Filters
{
    public class BlogAuthAttribute:AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["Userid"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary()
                {
                    { "controller","User"},
                    { "action","Login"}
                });
            }
        }
    }
}