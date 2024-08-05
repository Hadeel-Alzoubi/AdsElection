using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectElection.Models;

namespace ProjectElection.Controllers
{
    public class payments1Controller : Controller
    {
        private ElectionEntities1 db = new ElectionEntities1();

        // GET: payments1
        public ActionResult Index()
        {
            return View(db.payments.ToList());
        }

        // GET: payments1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            payment payment = db.payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: payments1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: payments1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,email,name,cardnumber,cvv")] payment payment)
        {
            if (ModelState.IsValid)
            {
                db.payments.Add(payment);
                db.SaveChanges();
                return RedirectToAction("Success");
            }

            return View("Success");
            
        }


        public ActionResult Success()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Success(FormCollection form)
        {
            
            return RedirectToAction("Index" ,"Home");
        }
    }
}
