﻿using System.Web;
using System.Web.Mvc;

namespace NguyenGiaHuy.SachOnline
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
