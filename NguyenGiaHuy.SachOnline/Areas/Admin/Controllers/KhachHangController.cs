using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using NguyenGiaHuy.SachOnline.Models;

namespace NguyenGiaHuy.SachOnline.Areas.Admin.Controllers
{
    public class KhachHangController : Controller
    {
        // Khởi tạo DbContext
        private SachOnline1Entities1 db = new SachOnline1Entities1();

        // GET: Admin/KhachHang
        public ActionResult QuanLyKhachHang()
        {
            var khachHangs = db.KHACHHANGs.ToList();
            return View(khachHangs);
        }

        // GET: Admin/KhachHang/Edit/5
        public ActionResult Edit(int id)
        {
            var kh = db.KHACHHANGs.Find(id);
            if (kh == null)
                return HttpNotFound();

            return View(kh);
        }

        // POST: Admin/KhachHang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KHACHHANG model, string PhanQuyen)
        {
            var kh = db.KHACHHANGs.Find(model.KhachHangID);
            if (kh != null)
            {
                kh.TenKhachHang = model.TenKhachHang;
                kh.DiaChi = model.DiaChi;
                kh.Email = model.Email;
                kh.SoDienThoai = model.SoDienThoai;
                kh.TenDN = model.TenDN;
                kh.MatKhau = model.MatKhau;

                // Nếu chọn phân quyền là Admin thì thêm tài khoản này vào bảng ADMIN
                if (PhanQuyen == "Admin")
                {
                    bool tonTai = db.ADMINs.Any(a => a.Username == model.TenDN);
                    if (!tonTai)
                    {
                        var admin = new ADMIN
                        {
                            Username = model.TenDN,
                            Password = model.MatKhau
                        };
                        db.ADMINs.Add(admin);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("QuanLyKhachHang");
            }

            return View(model);
        }

        // GET: Admin/KhachHang/Delete/5
        public ActionResult Delete(int id)
        {
            var kh = db.KHACHHANGs.Find(id);
            if (kh != null)
            {
                db.KHACHHANGs.Remove(kh);
                db.SaveChanges();
            }
            return RedirectToAction("QuanLyKhachHang");
        }
    }
}
