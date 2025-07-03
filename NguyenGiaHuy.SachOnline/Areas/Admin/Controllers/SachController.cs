using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenGiaHuy.SachOnline.Models;
using System.IO;
using NguyenGiaHuy.SachOnline.Models.ViewModels;

namespace NguyenGiaHuy.SachOnline.Areas.Admin.Controllers
{
    public class SachController : Controller
    {
        SachOnline1Entities1 db = new SachOnline1Entities1();

        // GET: Admin/Sach
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            var sachList = db.SACHes.OrderBy(s => s.SachID).ToList();
            int totalRow = sachList.Count();
            var pagedSach = sachList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRow / pageSize);

            return View(pagedSach);
        }

        // GET: Admin/Sach/Details
        public ActionResult Details(int id)
        {
            var sach = db.SACHes.SingleOrDefault(s => s.SachID == id);
            if (sach == null) return HttpNotFound();

            var chuDe = db.CHUDEs.FirstOrDefault(cd => cd.ChuDeID == sach.ChuDeID);
            var nxb = db.NHAXUATBANs.FirstOrDefault(n => n.NhaXuatBanID == sach.NhaXuatBanID);

            var viewModel = new SachDetailsViewModel
            {
                Sach = sach,
                TenChuDe = chuDe != null ? chuDe.TenChuDe : "Không rõ",
                TenNXB = nxb != null ? nxb.TenNhaXuatBan : "Không rõ"
            };

            return View(viewModel);
        }

        // GET: Admin/Sach/Create
        public ActionResult Create()
        {
            ViewBag.ChuDeID = new SelectList(db.CHUDEs, "ChuDeID", "TenChuDe");
            ViewBag.NhaXuatBanID = new SelectList(db.NHAXUATBANs, "NhaXuatBanID", "TenNhaXuatBan");
            ViewBag.TacGiaID = new SelectList(db.TACGIAs, "TacGiaID", "TenTacGia");
            return View();
        }

        // POST: Admin/Sach/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SACH sach, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(uploadFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    uploadFile.SaveAs(path);
                    sach.anhSP = fileName;
                }

                sach.NgayCapNhat = DateTime.Now;
                db.SACHes.Add(sach);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu lỗi -> nạp lại SelectList
            ViewBag.ChuDeID = new SelectList(db.CHUDEs, "ChuDeID", "TenChuDe", sach.ChuDeID);
            ViewBag.NhaXuatBanID = new SelectList(db.NHAXUATBANs, "NhaXuatBanID", "TenNhaXuatBan", sach.NhaXuatBanID);
            ViewBag.TacGiaID = new SelectList(db.TACGIAs, "TacGiaID", "TenTacGia", sach.TacGiaID);
            return View(sach);
        }

        // GET: Admin/Sach/Edit
        public ActionResult Edit(int id)
        {
            var sach = db.SACHes.SingleOrDefault(s => s.SachID == id);
            if (sach == null) return HttpNotFound();

            ViewBag.ChuDeID = new SelectList(db.CHUDEs, "ChuDeID", "TenChuDe", sach.ChuDeID);
            ViewBag.NhaXuatBanID = new SelectList(db.NHAXUATBANs, "NhaXuatBanID", "TenNhaXuatBan", sach.NhaXuatBanID);
            ViewBag.TacGiaID = new SelectList(db.TACGIAs, "TacGiaID", "TenTacGia", sach.TacGiaID);

            return View(sach);
        }

        // POST: Admin/Sach/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SACH updatedSach, HttpPostedFileBase uploadFile)
        {
            var sachInDb = db.SACHes.SingleOrDefault(s => s.SachID == updatedSach.SachID);
            if (sachInDb == null) return HttpNotFound();

            sachInDb.TenSach = updatedSach.TenSach;
            sachInDb.Mota = updatedSach.Mota;
            sachInDb.MoTaNgan = updatedSach.MoTaNgan;
            sachInDb.SoLuong = updatedSach.SoLuong;
            sachInDb.GiaBan = updatedSach.GiaBan;
            sachInDb.NgayCapNhat = DateTime.Now;
            sachInDb.ChuDeID = updatedSach.ChuDeID;
            sachInDb.NhaXuatBanID = updatedSach.NhaXuatBanID;
            sachInDb.TacGiaID = updatedSach.TacGiaID;

            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(uploadFile.FileName);
                string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                uploadFile.SaveAs(path);
                sachInDb.anhSP = fileName;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Admin/Sach/Delete
        public ActionResult Delete(int id)
        {
            var sach = db.SACHes.SingleOrDefault(s => s.SachID == id);
            if (sach == null) return HttpNotFound();

            db.SACHes.Remove(sach);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
