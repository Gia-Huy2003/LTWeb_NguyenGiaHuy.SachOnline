using System.Web;
using System.Web.Mvc;

namespace NguyenGiaHuy.SachOnline.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var admin = Session["TaiKhoanAdmin"];
            if (admin == null) // Chưa đăng nhập
            {
                filterContext.Result = RedirectToAction("Login", "Home", new { area = "Admin" });
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
