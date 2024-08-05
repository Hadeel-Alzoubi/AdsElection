using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectElection.Models;

namespace ProjectElection.Controllers
{
    public class TempAdsController : Controller
    {
        private ElectionEntities1 db = new ElectionEntities1();

        // GET: TempAds
        public ActionResult Index()
        {
            var tempAds = db.TempAds.Include(t => t.Ad);
            return View(tempAds.ToList());
        }

        // GET: TempAds/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempAd tempAd = db.TempAds.Find(id);
            if (tempAd == null)
            {
                return HttpNotFound();
            }
            return View(tempAd);
        }

        // GET: TempAds/Create
        public ActionResult Create()
        {
            ViewBag.id = new SelectList(db.Ads, "id", "name");
            return View();
        }

        // POST: TempAds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "id,name,listname,electionarea,image")] TempAd tempAd, HttpPostedFileBase upload)
        {

            var fileName = Path.GetFileName(upload.FileName);
            var path = Path.Combine(Server.MapPath("~/Images/"), fileName);


            upload.SaveAs(path);
            tempAd.image = fileName;

            if (ModelState.IsValid)
            {
                db.TempAds.Add(tempAd);
                db.SaveChanges();
                return RedirectToAction("Payment","Home");
            }

            ViewBag.id = new SelectList(db.Ads, "id", "name", tempAd.id);
            db.SaveChanges();
            return View();
        }

        // GET: TempAds/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempAd tempAd = db.TempAds.Find(id);
            if (tempAd == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = new SelectList(db.Ads, "id", "name", tempAd.id);
            return View(tempAd);
        }

        // POST: TempAds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,listname,electionarea,image")] TempAd tempAd)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tempAd).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id = new SelectList(db.Ads, "id", "name", tempAd.id);
            return View(tempAd);
        }

        // GET: TempAds/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempAd tempAd = db.TempAds.Find(id);
            if (tempAd == null)
            {
                return HttpNotFound();
            }
            return View(tempAd);
        }

        // POST: TempAds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TempAd tempAd = db.TempAds.Find(id);
            db.TempAds.Remove(tempAd);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
