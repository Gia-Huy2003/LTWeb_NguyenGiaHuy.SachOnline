using System;
using System.Linq;
using System.Web.Mvc;
using NguyenGiaHuy.SachOnline.Models;
using NguyenGiaHuy.SachOnline.Services;
using System.Data.Entity;

namespace NguyenGiaHuy.SachOnline.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // Context đúng
        SachOnline1Entities1 db = new SachOnline1Entities1();
        SachOnline1Entities1 dbSachOnlineDataContext = new SachOnline1Entities1();

        // Danh sách tất cả đơn hàng
        public ActionResult QuanLyDonHang(string filter)
        {
            var dsDon = db.DONDATHANGs.Include(d => d.KHACHHANG).AsQueryable();

            if (filter == "week")
            {
                var startOfWeek = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek);
                dsDon = dsDon.Where(d => d.NgayDat >= startOfWeek);
            }
            else if (filter == "month")
            {
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dsDon = dsDon.Where(d => d.NgayDat >= startOfMonth);
            }

            var list = dsDon.OrderByDescending(d => d.NgayDat).ToList();
            return View(list);
        }

        // Action duyệt đơn hàng
        public ActionResult DuyetDonHang(int id)
        {
            var dh = dbSachOnlineDataContext.DONDATHANGs.Find(id);
            if (dh != null && dh.TrangThaiDonHang == "Đang xử lý")
            {
                dh.TrangThaiDonHang = "Chờ giao";
                dbSachOnlineDataContext.SaveChanges();

                // ✅ Gửi email xác nhận khi duyệt
                GuiEmailXacNhan(dh);
            }

            return RedirectToAction("QuanLyDonHang");
        }

        private void GuiEmailXacNhan(DONDATHANG dh)
        {
            var kh = dh.KHACHHANG;
            var toEmail = kh.Email;
            var subject = "Đơn hàng của bạn đã được duyệt - SachOnline";

            var body = $@"
        <h3>Xin chào {kh.TenKhachHang},</h3>
        <p>Đơn hàng của bạn với mã <strong>{dh.MaDonHang}</strong> đã được <strong>duyệt</strong> vào lúc {DateTime.Now:HH:mm dd/MM/yyyy}.</p>
        <p><strong>Phương thức thanh toán:</strong> {(dh.PhuongThucThanhToan == "VNPay" ? "Chuyển khoản VNPay" : "Thanh toán khi nhận hàng")}</p>
        <p><strong>Địa chỉ giao hàng:</strong> {dh.DiaChiGiao}</p>
        <p>Chúng tôi sẽ giao hàng đến bạn trong thời gian sớm nhất.</p>
        <br/>
        <p>Trân trọng,</p>
        <p><strong>SachOnline</strong></p>
    ";

            var emailService = new EmailService();
            emailService.SendOrderConfirmationEmail(toEmail, subject, body);
        }
    }
}
