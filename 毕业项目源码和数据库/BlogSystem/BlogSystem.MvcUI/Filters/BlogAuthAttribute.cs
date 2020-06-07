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
            //if (filterContext.HttpContext.Session["Userid"] == null)
            //{
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary()
            //    {
            //        { "controller","User"},
            //        { "action","Login"}
            //    });
            //}

            if (filterContext.HttpContext.Request.Cookies["LoginName"]!=null&&
                filterContext.HttpContext.Session["Logname"]==null)  //cookie不为空，session为空时，将用户数据同步到session中
            {
                
                filterContext.HttpContext.Session["Logname"] = filterContext.HttpContext.Request.Cookies["LoginName"].Value;
                filterContext.HttpContext.Session["Userid"] = filterContext.HttpContext.Request.Cookies["Userid"].Value;
            }
            //如果都为空则回到登录界面
            if (filterContext.HttpContext.Request.Cookies["LoginName"]==null ||
                filterContext.HttpContext.Session["Logname"]==null)
            {
                filterContext.Result=new RedirectToRouteResult(new RouteValueDictionary()
                {
                    {"Controller","User"},
                    {"Action","Login"}
                });
            }
        }
    }
}