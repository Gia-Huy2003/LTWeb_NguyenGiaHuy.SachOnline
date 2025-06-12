using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenGiaHuy.SachOnline.Models;

namespace SachOnline.Controllers
{
    public class GioHangController : Controller
    {
        SachOnline1Entities1 dbSachOnlineDataContext;

        public GioHangController()
        {
            dbSachOnlineDataContext = new SachOnline1Entities1();
        }

        public ActionResult ThemGioHang(int ms, string url)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.Find(n => n.iSachID == ms);
            if (sp == null)
            {
                sp = new GioHang(ms);
                lstGioHang.Add(sp);
                TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng.";
            }
            else
            {
                sp.iSoLuong++;
            }

            return Redirect(url);
        }

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
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.dTongTien);
            }
            return dTongTien;
        }

        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
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
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iSachID == iSachID);
            if (sp != null)
            {
                lstGioHang.RemoveAll(n => n.iSachID == iSachID);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "SachOnline");
                }
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult CapNhatGioHang(int iSachID, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iSachID == iSachID);
            if (sp != null)
            {
                sp.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "SachOnline");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "SachOnline");
            }

            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "SachOnline");
            }

            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            DONDATHANG ddh = new DONDATHANG();
            List<GioHang> lstGioHang = LayGioHang();
            ddh.KhachHangID = kh.KhachHangID;
            ddh.NgayDat = DateTime.Now;
            var NgayGiao = String.Format("{0:MM/dd/yyyy}", f["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(NgayGiao);
            ddh.TinhTrangDonHang = false;
            ddh.DaThanhToan = false;
            dbSachOnlineDataContext.DONDATHANGs.Add(ddh);
            dbSachOnlineDataContext.SaveChanges();

            foreach (var item in lstGioHang)
            {
                CHITIETDATHANG ct = new CHITIETDATHANG();
                ct.DonDatHangID = ddh.DonDatHangID;
                ct.SachID = item.iSachID;
                ct.SoLuong = item.iSoLuong;
                ct.GiaTien = item.dGiaTien;
                dbSachOnlineDataContext.CHITIETDATHANGs.Add(ct);
            }
            dbSachOnlineDataContext.SaveChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }
    }
}
