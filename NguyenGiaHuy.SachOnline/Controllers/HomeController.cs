using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenGiaHuy.SachOnline.Models; // ✅ DÒNG NÀY CẦN THÊM

namespace NguyenGiaHuy.SachOnline.Controllers
{
    public class HomeController : Controller
    {
        SachOnline1Entities1 db = new SachOnline1Entities1();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View("~/Areas/Admin/Views/Home/Login.cshtml");
        }

        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            // Kiểm tra tài khoản Admin
            var admin = db.ADMINs.SingleOrDefault(a => a.Username == Username && a.Password == Password);
            if (admin != null)
            {
                Session["AdminUsername"] = admin.Username;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            // Kiểm tra tài khoản Khách hàng
            var user = db.KHACHHANGs.SingleOrDefault(u => u.TenDN == Username && u.MatKhau == Password);
            if (user != null)
            {
                Session["User"] = user;
                Session["UserName"] = user.TenKhachHang;
                return RedirectToAction("Index", "SachOnline");
            }

            // Sai thông tin
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        public ActionResult Logout()
        {
            Session["AdminUsername"] = null;
            Session["User"] = null;
            Session["UserName"] = null;
            return RedirectToAction("Login");
        }
    }
}
