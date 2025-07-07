using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NguyenGiaHuy.SachOnline.Models;

namespace NguyenGiaHuy.SachOnline.Controllers
{
    public class SachOnlineController : Controller
    {
        SachOnline1Entities1 db = new SachOnline1Entities1();
        private SachOnline1Entities1 data;

        public SachOnlineController()
        {
            data = new SachOnline1Entities1();
        }

        private List<SACH> LaySachMoi(int count)
        {
            return data.SACHes.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }

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

        public ActionResult IndexAdmin()
        {
            if (Session["TaiKhoanAdmin"] == null)
                return RedirectToAction("DangNhap");

            return View();
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            ViewBag.ThongBao = TempData["ThongBao"];
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
                var admin = data.ADMINs.FirstOrDefault(a => a.Username == sTenDN && a.Password == sMatkhau);
                if (admin != null)
                {
                    Session["TaiKhoanAdmin"] = admin;
                    return RedirectToAction("IndexAdmin");
                }

                KHACHHANG kh = data.KHACHHANGs.FirstOrDefault(n => n.TenDN == sTenDN && n.MatKhau == sMatkhau);
                if (kh != null)
                {
                    Session["TaiKhoan"] = kh;
                    return RedirectToAction("Index");
                }

                ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không hợp lệ";
            }

            return View();
        }
        public ActionResult LocSach(string loaiLoc)
        {
            var sach = db.SACHes.AsQueryable();

            switch (loaiLoc)
            {
                case "thapcao":
                    sach = sach.OrderBy(s => s.GiaBan);
                    break;
                case "caothap":
                    sach = sach.OrderByDescending(s => s.GiaBan);
                    break;
                case "khuyenmai":
                    sach = sach.Where(s => s.GiaKhuyenMai != null && s.GiaKhuyenMai < s.GiaBan);
                    break;
                default:
                    sach = sach.OrderByDescending(s => s.NgayCapNhat); // mặc định
                    break;
            }

            return View("Index", sach.ToList()); // Dùng lại View Index.cshtml
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
        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            Session["TaiKhoanAdmin"] = null;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult TimKiem(string keyword, int page = 1, int pageSize = 6)
        {
            var allResults = db.SACHes
                .Where(s => s.TenSach.Contains(keyword))
                .OrderByDescending(s => s.NgayCapNhat)
                .ToList();

            int total = allResults.Count();
            var ketQuaPhanTrang = allResults
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TuKhoa = keyword;
            ViewBag.Trang = page;
            ViewBag.TongTrang = (int)Math.Ceiling((double)total / pageSize);

            return View(ketQuaPhanTrang);
        }
        public ActionResult TimKiem(string keyword)
        {
            var kq = data.SACHes.Where(s => s.TenSach.Contains(keyword)).ToList();
            ViewBag.TuKhoa = keyword;
            return View("KetQuaTimKiem", kq);
        }
        public ActionResult ChuDePartial()
        {
            var chudeList = data.CHUDEs.ToList();
            return PartialView(chudeList);
        }

        public ActionResult NhaXuatBanPartial()
        {
            var listNhaXuatBan = data.NHAXUATBANs.ToList();
            return PartialView(listNhaXuatBan);
        }

        public ActionResult SachBanNhieuPartial()
        {
            return View();
        }

        public ActionResult SachTheoChuDe(int id)
        {
            var sachList = data.SACHes.Where(s => s.ChuDeID == id).ToList();

            if (sachList.Count < 4)
            {
                var soCanThem = 4 - sachList.Count;
                var sachBoSung = data.SACHes
                                    .Where(s => s.ChuDeID != id)
                                    .Take(soCanThem)
                                    .ToList();

                sachList.AddRange(sachBoSung);
            }

            return View(sachList);
        }

        public ActionResult SachTheoNhaXuatBan(int id)
        {
            var sach = data.SACHes.Where(s => s.NhaXuatBanID == id).ToList();
            return View(sach);
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
            List<int> cart = Session["Cart"] as List<int> ?? new List<int>();
            cart.Add(id);
            Session["Cart"] = cart;
            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";
            return RedirectToAction("Index", "SachOnline");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Giới thiệu về Bookstore";
            return View();
        }
    }
}
