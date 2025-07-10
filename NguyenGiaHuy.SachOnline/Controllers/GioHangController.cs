using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;
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

        [HttpPost]
        public ActionResult ThemGioHang(int ms, int soLuong, string url)
        {
            var sach = dbSachOnlineDataContext.SACHes.SingleOrDefault(s => s.SachID == ms);
            if (sach == null)
            {
                return HttpNotFound();
            }

            var lstGioHang = LayGioHang();
            var sp = lstGioHang.FirstOrDefault(n => n.iSachID == ms);

            if (soLuong < 1)
            {
                TempData["ErrorMessage"] = "Số lượng phải từ 1 trở lên.";
                return Redirect(url);
            }

            if (sp == null)
            {
                if (soLuong > sach.SoLuong)
                {
                    TempData["ErrorMessage"] = $"Chỉ còn {sach.SoLuong} sản phẩm trong kho!";
                    return Redirect(url);
                }
                lstGioHang.Add(new GioHang(ms) { iSoLuong = soLuong });
            }
            else
            {
                int tongSoLuong = sp.iSoLuong + soLuong;

                if (tongSoLuong > sach.SoLuong)
                {
                    TempData["ErrorMessage"] = $"Tổng số lượng trong giỏ đã vượt quá tồn kho! Còn lại {sach.SoLuong} sản phẩm.";
                    return Redirect(url);
                }

                sp.iSoLuong += soLuong;
            }

            TempData["SuccessMessage"] = $"Đã thêm {soLuong} sản phẩm vào giỏ hàng.";
            return Redirect(url);
        }

        public ActionResult GioHang()
        {
            if (Session["TaiKhoan"] == null)
                return RedirectToAction("DangNhap", "SachOnline");

            var lstGioHang = LayGioHang();
            if (!lstGioHang.Any())
                return RedirectToAction("Index", "SachOnline");

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

        [HttpPost]
        public ActionResult CapNhatGioHang(int iSachID, FormCollection f)
        {
            var lstGioHang = LayGioHang();
            var sp = lstGioHang.FirstOrDefault(n => n.iSachID == iSachID);
            var sach = dbSachOnlineDataContext.SACHes.SingleOrDefault(s => s.SachID == iSachID);

            if (sp != null && sach != null)
            {
                int soLuongMoi = int.Parse(f["txtSoLuong"]);

                if (soLuongMoi < 1)
                {
                    TempData["ErrorMessage"] = "Số lượng phải từ 1 trở lên.";
                }
                else if (soLuongMoi > sach.SoLuong)
                {
                    TempData["ErrorMessage"] = $"Chỉ còn {sach.SoLuong} sản phẩm trong kho!";
                }
                else
                {
                    sp.iSoLuong = soLuongMoi;
                }
            }

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
            string diaChiGiao = f["DiaChi"];
            string phuongThuc = f["PhuongThucThanhToan"];

            // Tạo mã đơn hàng dạng "DHxxxxxx"
            Random rnd = new Random();
            string maDon = "DH" + rnd.Next(100000, 999999);

            // Tạo đơn đặt hàng
            DONDATHANG ddh = new DONDATHANG
            {
                KhachHangID = kh.KhachHangID,
                NgayDat = DateTime.Now,
                DiaChiGiao = diaChiGiao,
                TinhTrangDonHang = false,
                DaThanhToan = phuongThuc == "VNPay",
                PhuongThucThanhToan = phuongThuc,
                MaDonHang = maDon,

                TrangThaiDonHang = "Đang xử lý" // hoặc trạng thái mặc định nào bạn muốn
            };

            dbSachOnlineDataContext.DONDATHANGs.Add(ddh);
            dbSachOnlineDataContext.SaveChanges();

            // Thêm các chi tiết đặt hàng
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

            // Xóa giỏ hàng khỏi session
            Session["GioHang"] = null;

            // Nếu chọn chuyển khoản thì chuyển qua trang mã QR
            if (phuongThuc == "VNPay")
                return RedirectToAction("ThanhToanVNPay", new { id = ddh.DonDatHangID });

            // Nếu COD thì hiển thị xác nhận
            TempData["MaDonHang"] = ddh.MaDonHang;
            return RedirectToAction("XacNhanDonHang");
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }

        public ActionResult DonHangCuaToi(int? page)
        {
            if (Session["TaiKhoan"] == null)
                return RedirectToAction("DangNhap", "SachOnline");

            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            var donhang = dbSachOnlineDataContext.DONDATHANGs
                .Where(d => d.KhachHangID == kh.KhachHangID)
                .OrderByDescending(d => d.NgayDat);

            int pageSize = 9; // số đơn hàng trên 1 trang
            int pageNumber = (page ?? 1); // trang hiện tại

            return View(donhang.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult XacNhanDaNhanHang(int id)
        {
            var donHang = dbSachOnlineDataContext.DONDATHANGs.FirstOrDefault(d => d.DonDatHangID == id);
            if (donHang == null) return HttpNotFound();

            donHang.TinhTrangDonHang = true;
            donHang.DaThanhToan = true;
            donHang.NgayGiao = DateTime.Now;

            // ❗ Đặt trước SaveChanges để được lưu vào DB
            donHang.TrangThaiDonHang = "Đã nhận";

            dbSachOnlineDataContext.SaveChanges();

            TempData["ThongBao"] = $"Đã xác nhận đơn hàng #{id} là đã giao thành công.";
            return RedirectToAction("DonHangCuaToi");
        }

        public ActionResult ThanhToanVNPay(int id)
        {
            var ddh = dbSachOnlineDataContext.DONDATHANGs.FirstOrDefault(d => d.DonDatHangID == id);
            if (ddh == null) return HttpNotFound();

            ViewBag.TongTien = dbSachOnlineDataContext.CHITIETDATHANGs
                .Where(c => c.DonDatHangID == id)
                .Sum(c => c.GiaTien * c.SoLuong);

            return View("ThanhToanTrucTuyen", ddh);
        }
    }
}
