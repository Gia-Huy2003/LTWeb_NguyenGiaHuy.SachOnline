using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NguyenGiaHuy.SachOnline.Models;

namespace NguyenGiaHuy.SachOnline.Areas.Admin.Controllers
{
    public class NhaXuatBanController : Controller
    {
        private SachOnline1Entities1 db = new SachOnline1Entities1();

        // GET: Admin/NhaXuatBan
        public ActionResult Index()
        {
            var dsNXB = db.NHAXUATBANs.ToList();
            return View("QuanLyNhaXuatBan", dsNXB);
        }

        // GET: Admin/NhaXuatBan/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhaXuatBan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NHAXUATBAN nxb)
        {
            if (ModelState.IsValid)
            {
                db.NHAXUATBANs.Add(nxb);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nxb);
        }

        // GET: Admin/NhaXuatBan/Edit/5
        public ActionResult Edit(int id)
        {
            var nxb = db.NHAXUATBANs.SingleOrDefault(x => x.NhaXuatBanID == id);
            if (nxb == null) return HttpNotFound();
            return View(nxb);
        }

        // POST: Admin/NhaXuatBan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NHAXUATBAN nxb)
        {
            if (ModelState.IsValid)
            {
                var nxbInDb = db.NHAXUATBANs.SingleOrDefault(x => x.NhaXuatBanID == nxb.NhaXuatBanID);
                if (nxbInDb == null) return HttpNotFound();

                nxbInDb.TenNhaXuatBan = nxb.TenNhaXuatBan;
                nxbInDb.DiaChi = nxb.DiaChi;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nxb);
        }

        // GET: Admin/NhaXuatBan/Delete/5
        public ActionResult Delete(int id)
        {
            var nxb = db.NHAXUATBANs.SingleOrDefault(x => x.NhaXuatBanID == id);
            if (nxb == null) return HttpNotFound();

            db.NHAXUATBANs.Remove(nxb);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
