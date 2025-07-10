using System.Web.Mvc;

namespace NguyenGiaHuy.SachOnline.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // Trang chủ admin
        public ActionResult Index()
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                // Nếu chưa đăng nhập, chuyển về trang đăng nhập ngoài khu vực Admin
                return RedirectToAction("DangNhap", "SachOnline", new { area = "" });
            }

            // Nếu đã đăng nhập, chuyển đến trang quản lý sách
            return RedirectToAction("Index", "Sach");
        }

        // Đăng xuất admin
        public ActionResult Logout()
        {
            Session["TaiKhoanAdmin"] = null;
            return RedirectToAction("DangNhap", "SachOnline", new { area = "" });
        }
    }
}
