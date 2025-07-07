using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenGiaHuy.SachOnline.Models;
using System.Data.Entity;
using System.Net;



namespace NguyenGiaHuy.SachOnline.Areas.Admin.Controllers
{
    public class ChuDeController : Controller
    {
        private readonly SachOnline1Entities1 db = new SachOnline1Entities1();

        // GET: Admin/ChuDe
        public ActionResult QuanLyChuDe()
        {
            var chudeList = db.CHUDEs.ToList();
            return View(chudeList);
        }

        // GET: Admin/ChuDe/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ChuDe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CHUDE chude)
        {
            if (ModelState.IsValid)
            {
                db.CHUDEs.Add(chude);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chude);
        }

        // GET: Admin/ChuDe/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            CHUDE cd = db.CHUDEs.Find(id);
            if (cd == null) return HttpNotFound();

            return View(cd);
        }

        // POST: Admin/ChuDe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CHUDE chude)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chude).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chude);
        }

        // GET: Admin/ChuDe/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            CHUDE cd = db.CHUDEs.Find(id);
            if (cd == null) return HttpNotFound();

            db.CHUDEs.Remove(cd);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
