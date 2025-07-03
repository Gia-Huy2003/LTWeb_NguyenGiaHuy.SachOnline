using System;
using System.Linq;
using System.Web.Mvc;
using NguyenGiaHuy.SachOnline.Models;
using NguyenGiaHuy.SachOnline.Models.ViewModels;
using System.Data.Entity;

namespace NguyenGiaHuy.SachOnline.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        private SachOnline1Entities1 db = new SachOnline1Entities1();

        public ActionResult DoanhThu(DateTime? tuNgay, DateTime? denNgay)
        {
            var query = db.DONDATHANGs
                .Where(d => d.TrangThaiDonHang == "Đã nhận");

            if (tuNgay.HasValue)
                query = query.Where(d => d.NgayDat >= tuNgay.Value);
            if (denNgay.HasValue)
                query = query.Where(d => d.NgayDat <= denNgay.Value);

            // Gọi ToList() trước để truy vấn xong
            var danhSach = query.Include("CHITIETDATHANGs").ToList();

            var thongKe = danhSach.Select(d => new ThongKeDoanhThuViewModel
            {
                DonDatHangID = d.DonDatHangID,
                NgayDat = d.NgayDat,
                TongTien = (decimal)(d.CHITIETDATHANGs.Sum(ct => ct.SoLuong * (ct.GiaTien ?? 0)))
            })
            .OrderByDescending(d => d.NgayDat)
            .ToList();

            ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");
            ViewBag.TongDoanhThu = thongKe.Sum(d => d.TongTien);

            return View(thongKe);
        }
    }
}
