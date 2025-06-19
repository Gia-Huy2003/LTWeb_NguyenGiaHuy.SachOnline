using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenGiaHuy.SachOnline.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["TaiKhoanAdmin"] == null)
                return RedirectToAction("DangNhap", "SachOnline");

            return View();
        }

        public ActionResult Logout()
        {
            Session["TaiKhoanAdmin"] = null;
            return RedirectToAction("DangNhap", "SachOnline", new { area = "" });
        }
    }
}