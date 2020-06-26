﻿using System.Web;
using System.Web.Mvc;
using BlogSystem.MvcUI.Filters;

namespace BlogSystem.MvcUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
