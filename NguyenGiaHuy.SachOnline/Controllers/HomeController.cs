using System.Linq;
using System.Web.Mvc;
using NguyenGiaHuy.SachOnline.Models;

namespace NguyenGiaHuy.SachOnline.Controllers
{
    public class HomeController : Controller
    {
        SachOnline1Entities1 db = new SachOnline1Entities1();

        public ActionResult Index()
        {
            var ds = db.SACHes.ToList();

            var vm = new SachListViewModel
            {
                DanhSachSach = ds
            };

            return View(vm);
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
            var admin = db.ADMINs.SingleOrDefault(a => a.Username == Username && a.Password == Password);
            if (admin != null)
            {
                Session["AdminUsername"] = admin.Username;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View("~/Areas/Admin/Views/Home/Login.cshtml");
        }

        public ActionResult Logout()
        {
            Session["AdminUsername"] = null;
            Session["User"] = null;
            Session["UserName"] = null;
            return RedirectToAction("Login");
        }

        public ActionResult DoiMatKhau()
        {
            return View("~/Views/SachOnline/DoiMatKhau.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(DoiMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin hợp lệ.";
                return View("~/Views/SachOnline/DoiMatKhau.cshtml", model);
            }

            var kh = db.KHACHHANGs.SingleOrDefault(k => k.TenDN == model.TenDN && k.MatKhau == model.MatKhauCu);
            if (kh == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu cũ không đúng.");
                return View("~/Views/SachOnline/DoiMatKhau.cshtml", model);
            }

            if (model.MatKhauMoi != model.XacNhanMatKhauMoi)
            {
                ModelState.AddModelError("", "Mật khẩu mới và xác nhận không khớp.");
                return View("~/Views/SachOnline/DoiMatKhau.cshtml", model);
            }

            // ✅ Cập nhật mật khẩu
            kh.MatKhau = model.MatKhauMoi;
            db.Entry(kh).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            // ✅ Thoát session và hiển thị thông báo
            Session["User"] = null;
            Session["UserName"] = null;
            TempData["ThongBao"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.";

            return RedirectToAction("DangNhap", "SachOnline");
        }
    }
}
