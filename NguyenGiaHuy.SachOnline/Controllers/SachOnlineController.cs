using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using NguyenGiaHuy.SachOnline.Models;

namespace SachOnline.Controllers
{
    public class SachOnlineController : Controller
    {
        private SachOnline1Entities1 data;

        public SachOnlineController()
        {
            data = new SachOnline1Entities1();
        }

        private List<SACH> LaySachMoi(int count)
        {
            return data.SACHes.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }

        // GET: SachOnline
        public ActionResult Index(int page = 1)
        {
            int pageSize = 6;
            var allBooks = data.SACHes.OrderByDescending(s => s.NgayCapNhat);
            var sachMoi = allBooks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new SachListViewModel
            {
                Saches = sachMoi,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = allBooks.Count(),
                    UrlPage = p => Url.Action("Index", new { page = p })
                }
            };

            return View(model);
        }

        public ActionResult ChuDePartial()
        {
            var chudeList = data.CHUDEs.ToList();
            return PartialView(chudeList);
        }

        public ActionResult NhaXuatBanPartial()
        {
            var listNhaXuatBan = from cd in data.NHAXUATBANs select cd;
            return PartialView(listNhaXuatBan);
        }

        public ActionResult SachBanNhieuPartial()
        {
            return View();
        }

        public ActionResult SachTheoChuDe(int id)
        {
            var sachList = data.SACHes.Where(s => s.ChuDeID == id).ToList();
            return View(sachList); // Trả về View SachTheoChuDe.cshtml
        }

        public ActionResult SachTheoNhaXuatBan(int id)
        {
            var sach = from s in data.SACHes where s.NhaXuatBanID == id select s;
            return View(sach);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Giới thiệu về Bookstore";
            return View();
        }


        public ActionResult BookDetail(int id)
        {
            var sach = data.SACHes.FirstOrDefault(s => s.SachID == id);

            if (sach == null)
            {
                return HttpNotFound();
            }

            return View(sach);
        }

        public ActionResult AddToCart(int id)
        {
            List<int> cart = Session["Cart"] as List<int>;
            if (cart == null)
            {
                cart = new List<int>();
            }
            cart.Add(id);
            Session["Cart"] = cart;
            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";
            return RedirectToAction("Index", "SachOnline");
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KHACHHANG kh)
        {
            var sTenKhachHang = collection["TenKhachHang"];
            var sDiaChi = collection["DiaChi"];
            var sTenDN = collection["TenDN"];
            var sMatkhau = collection["Matkhau"];
            var sMatkhauNhapLai = collection["MatKhauNL"];
            var sEmail = collection["Email"];
            var sSoDienThoai = collection["SoDienThoai"];

            if (String.IsNullOrEmpty(sTenKhachHang))
                ViewData["erro1"] = "Họ tên không được rỗng";
            else if (String.IsNullOrEmpty(sTenDN))
                ViewData["err2"] = "Tên đăng nhập không được rỗng";
            else if (String.IsNullOrEmpty(sMatkhau))
                ViewData["err3"] = "Phải nhập mật khẩu";
            else if (String.IsNullOrEmpty(sMatkhauNhapLai))
                ViewData["err4"] = "Phải nhập lại mật khẩu";
            else if (sMatkhau != sMatkhauNhapLai)
                ViewData["err4"] = "MK nhập lại không khớp";
            else if (String.IsNullOrEmpty(sEmail))
                ViewData["err5"] = "Email không được rỗng";
            else if (String.IsNullOrEmpty(sSoDienThoai))
                ViewData["err6"] = "Số điện thoại không được rỗng";
            else if (data.KHACHHANGs.Any(n => n.TenDN == sTenDN))
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại";
            else if (data.KHACHHANGs.Any(n => n.Email == sEmail))
                ViewBag.ThongBao = "Email này đã được sử dụng";
            else
            {
                // Gán giá trị
                kh.TenKhachHang = sTenKhachHang;
                kh.TenDN = sTenDN;
                kh.MatKhau = sMatkhau;
                kh.Email = sEmail;
                kh.DiaChi = sDiaChi;
                kh.SoDienThoai = sSoDienThoai;

                data.KHACHHANGs.Add(kh);
                data.SaveChanges();

                TempData["SuccessMessage"] = "Đăng ký thành công!";
                return RedirectToAction("DangNhap");
            }

            return View(kh);
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var sTenDN = collection["TenDN"];
            var sMatkhau = collection["Matkhau"];

            if (String.IsNullOrEmpty(sTenDN))
                ViewData["Err1"] = "Vui lòng nhập tên đăng nhập";
            else if (String.IsNullOrEmpty(sMatkhau))
                ViewData["Err2"] = "Vui lòng nhập mật khẩu";
            else
            {
                KHACHHANG kh = data.KHACHHANGs
                    .FirstOrDefault(n => n.TenDN == sTenDN && n.MatKhau == sMatkhau);
                if (kh != null)
                {
                    ViewBag.ThongBao = "Đăng nhập thành công vào hệ thống";
                    Session["TaiKhoan"] = kh;
                    return RedirectToAction("Index", "SachOnline");
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không hợp lệ";
                }
            }

            return View();
        }
    }
}
