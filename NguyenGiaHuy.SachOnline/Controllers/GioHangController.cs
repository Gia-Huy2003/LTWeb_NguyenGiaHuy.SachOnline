using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NguyenGiaHuy.SachOnline.Models;
using NguyenGiaHuy.SachOnline.Services;

namespace NguyenGiaHuy.SachOnline.Controllers
{
    public class GioHangController : Controller
    {
        SachOnline1Entities1 dbSachOnlineDataContext = new SachOnline1Entities1();

        private List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        private int TongSoLuong()
        {
            return LayGioHang().Sum(n => n.iSoLuong);
        }

        private double TongTien()
        {
            return LayGioHang().Sum(n => n.dTongTien);
        }

        public ActionResult ThemGioHang(int ms, string url)
        {
            var lstGioHang = LayGioHang();
            var sp = lstGioHang.FirstOrDefault(n => n.iSachID == ms);
            if (sp == null)
                lstGioHang.Add(new GioHang(ms));
            else
                sp.iSoLuong++;
            return Redirect(url);
        }

        public ActionResult GioHang()
        {
            // Nếu chưa đăng nhập thì chuyển sang trang Đăng Nhập
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "SachOnline");
            }

            var lstGioHang = LayGioHang();

            // Nếu giỏ hàng trống thì quay lại trang chủ
            if (!lstGioHang.Any())
            {
                return RedirectToAction("Index", "SachOnline");
            }

            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }


        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        public ActionResult XoaSPKhoiGioHang(int iSachID)
        {
            var lstGioHang = LayGioHang();
            lstGioHang.RemoveAll(n => n.iSachID == iSachID);
            return RedirectToAction("GioHang");
        }

        public ActionResult CapNhatGioHang(int iSachID, FormCollection f)
        {
            var lstGioHang = LayGioHang();
            var sp = lstGioHang.FirstOrDefault(n => n.iSachID == iSachID);
            if (sp != null) sp.iSoLuong = int.Parse(f["txtSoLuong"]);
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaGioHang()
        {
            LayGioHang().Clear();
            return RedirectToAction("Index", "SachOnline");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["GioHang"] == null)
                return RedirectToAction("DangNhap", "SachOnline");

            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(LayGioHang());
        }

        [HttpPost]
        [ActionName("DatHang")]
        public ActionResult XuLyDatHang(FormCollection f)
        {
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            DONDATHANG ddh = new DONDATHANG
            {
                KhachHangID = kh.KhachHangID,
                NgayDat = DateTime.Now,
                NgayGiao = DateTime.Parse(f["NgayGiao"]),
                TinhTrangDonHang = false,
                DaThanhToan = false
            };
            dbSachOnlineDataContext.DONDATHANGs.Add(ddh);
            dbSachOnlineDataContext.SaveChanges();

            foreach (var item in LayGioHang())
            {
                CHITIETDATHANG ct = new CHITIETDATHANG
                {
                    DonDatHangID = ddh.DonDatHangID,
                    SachID = item.iSachID,
                    SoLuong = item.iSoLuong,
                    GiaTien = item.dGiaTien
                };
                dbSachOnlineDataContext.CHITIETDATHANGs.Add(ct);
            }
            dbSachOnlineDataContext.SaveChanges();

            try
            {
                string email = kh.Email;
                string tieuDe = "Xác nhận đơn hàng từ SachOnline";
                string noiDung = $@"
                    <h3>Xin chào {kh.TenKhachHang}!</h3>
                    <p>Đơn hàng của bạn đã được tiếp nhận vào lúc {ddh.NgayDat:HH:mm dd/MM/yyyy}.</p>
                    <p>Tổng tiền: <strong>{TongTien():N0} VND</strong></p>
                    <p>Chúng tôi sẽ giao hàng trước ngày {ddh.NgayGiao:dd/MM/yyyy}.</p>";
                new EmailService().SendOrderConfirmationEmail(email, tieuDe, noiDung);
            }
            catch (Exception) { }

            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }

        public ActionResult DonHangCuaToi()
        {
            if (Session["TaiKhoan"] == null) return RedirectToAction("DangNhap", "SachOnline");
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            var donhang = dbSachOnlineDataContext.DONDATHANGs
                .Where(d => d.KhachHangID == kh.KhachHangID)
                .OrderByDescending(d => d.NgayDat)
                .ToList();
            return View(donhang);
        }

        public ActionResult XacNhanDaNhanHang(int id)
        {
            var donHang = dbSachOnlineDataContext.DONDATHANGs.FirstOrDefault(d => d.DonDatHangID == id);
            if (donHang == null) return HttpNotFound();

            donHang.TinhTrangDonHang = true;
            donHang.DaThanhToan = true;
            donHang.NgayGiao = DateTime.Now;
            dbSachOnlineDataContext.SaveChanges();

            TempData["ThongBao"] = $"Đã xác nhận đơn hàng #{id} là đã giao thành công.";
            return RedirectToAction("DonHangCuaToi");
        }
    }
}
