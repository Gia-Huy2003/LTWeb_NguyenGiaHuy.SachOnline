﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NguyenGiaHuy.SachOnline
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "SachOnline", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "NguyenGiaHuy.SachOnline.Controllers" } // <-- THÊM DÒNG NÀY
            );
        }
    }
}
