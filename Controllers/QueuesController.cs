using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BadmintonSvc.Models;

namespace BadmintonSvc.Controllers
{
    public class QueuesController : Controller
    {
        private BadmintonSvcContext db = new BadmintonSvcContext();

        // GET: Queues
        public ActionResult Index()
        {
            var queues = db.Queues.Include(q => q.Club).Include(q => q.Player).Include(q => q.QStatus).Include(q => q.Skillset);
            return View(queues.ToList());
        }

        // GET: Queues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Queue queue = db.Queues.Find(id);
            if (queue == null)
            {
                return HttpNotFound();
            }
            return View(queue);
        }

        // GET: Queues/Create
        public ActionResult Create()
        {
            ViewBag.ClubID = new SelectList(db.Clubs, "ClubID", "ClubName");
            ViewBag.PlayerID = new SelectList(db.Players, "PlayerID", "PlayerName");
            ViewBag.QStatusID = new SelectList(db.QStatus, "QStatusID", "StatusName");
            ViewBag.SkillsetID = new SelectList(db.Skillsets, "SkillsetID", "SkillsetName");
            return View();
        }

        // POST: Queues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QueueID,ClubID,PlayerID,SkillsetID,Score,PlayDateTime,QStatusID")] Queue queue)
        {
            if (ModelState.IsValid)
            {
                db.Queues.Add(queue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClubID = new SelectList(db.Clubs, "ClubID", "ClubName", queue.ClubID);
            ViewBag.PlayerID = new SelectList(db.Players, "PlayerID", "PlayerName", queue.PlayerID);
            ViewBag.QStatusID = new SelectList(db.QStatus, "QStatusID", "StatusName", queue.QStatusID);
            ViewBag.SkillsetID = new SelectList(db.Skillsets, "SkillsetID", "SkillsetName", queue.SkillsetID);
            return View(queue);
        }

        // GET: Queues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Queue queue = db.Queues.Find(id);
            if (queue == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClubID = new SelectList(db.Clubs, "ClubID", "ClubName", queue.ClubID);
            ViewBag.PlayerID = new SelectList(db.Players, "PlayerID", "PlayerName", queue.PlayerID);
            ViewBag.QStatusID = new SelectList(db.QStatus, "QStatusID", "StatusName", queue.QStatusID);
            ViewBag.SkillsetID = new SelectList(db.Skillsets, "SkillsetID", "SkillsetName", queue.SkillsetID);
            return View(queue);
        }

        // POST: Queues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QueueID,ClubID,PlayerID,SkillsetID,Score,PlayDateTime,QStatusID")] Queue queue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(queue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClubID = new SelectList(db.Clubs, "ClubID", "ClubName", queue.ClubID);
            ViewBag.PlayerID = new SelectList(db.Players, "PlayerID", "PlayerName", queue.PlayerID);
            ViewBag.QStatusID = new SelectList(db.QStatus, "QStatusID", "StatusName", queue.QStatusID);
            ViewBag.SkillsetID = new SelectList(db.Skillsets, "SkillsetID", "SkillsetName", queue.SkillsetID);
            return View(queue);
        }

        // GET: Queues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Queue queue = db.Queues.Find(id);
            if (queue == null)
            {
                return HttpNotFound();
            }
            return View(queue);
        }

        // POST: Queues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Queue queue = db.Queues.Find(id);
            db.Queues.Remove(queue);
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
