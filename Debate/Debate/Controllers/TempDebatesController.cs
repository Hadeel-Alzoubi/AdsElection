using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Debate.Models;

namespace Debate.Controllers
{
    public class TempDebatesController : Controller
    {
        private ElectionEntities db = new ElectionEntities();

        // GET: TempDebates
        public ActionResult Index()
        {
            var tempDebates = db.TempDebates.Include(t => t.Debate);
            return View(tempDebates.ToList());
        }

        // GET: TempDebates/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempDebate tempDebate = db.TempDebates.Find(id);
            if (tempDebate == null)
            {
                return HttpNotFound();
            }
            return View(tempDebate);
        }

        // GET: TempDebates/Create
        public ActionResult Create()
        {
            ViewBag.id = new SelectList(db.Debates, "id", "listname");
            return View();
        }

        // POST: TempDebates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,listname,areaelection,time,description")] TempDebate tempDebate)
        {
            if (ModelState.IsValid)
            {
                db.TempDebates.Add(tempDebate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id = new SelectList(db.Debates, "id", "listname", tempDebate.id);
            return View(tempDebate);
        }


        public ActionResult CreateDebate()
        {
            ViewBag.id = new SelectList(db.Debates, "id", "listname");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDebate([Bind(Include = "id,listname,areaelection,time,description,candidateName,phoneNo,email")] TempDebate tempDebate)
        {
            if (ModelState.IsValid)
            {
                db.TempDebates.Add(tempDebate);

                db.SaveChanges();
                return RedirectToAction("Success");
            }

            ViewBag.id = new SelectList(db.Debates, "id", "listname", tempDebate.id);
            return View(tempDebate);
        }


        public ActionResult Success()
        {
            var tempDebates = db.TempDebates.Include(t => t.Debate);
            return View(tempDebates.ToList());
        }
    }
}
