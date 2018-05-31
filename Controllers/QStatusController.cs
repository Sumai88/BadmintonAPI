using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BadmintonSvc.Models;

namespace BadmintonSvc.Controllers
{
    public class QStatusController : ApiController
    {
        private BadmintonSvcContext db = new BadmintonSvcContext();

        // GET: api/QStatus
        public IQueryable<QStatus> GetQStatus()
        {
            return db.QStatus;
        }

        // GET: api/QStatus/5
        [ResponseType(typeof(QStatus))]
        public IHttpActionResult GetQStatus(int id)
        {
            QStatus qStatus = db.QStatus.Find(id);
            if (qStatus == null)
            {
                return NotFound();
            }

            return Ok(qStatus);
        }

        // PUT: api/QStatus/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQStatus(int id, QStatus qStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != qStatus.QStatusID)
            {
                return BadRequest();
            }

            db.Entry(qStatus).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QStatusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/QStatus
        [ResponseType(typeof(QStatus))]
        public IHttpActionResult PostQStatus(QStatus qStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.QStatus.Add(qStatus);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = qStatus.QStatusID }, qStatus);
        }

        // DELETE: api/QStatus/5
        [ResponseType(typeof(QStatus))]
        public IHttpActionResult DeleteQStatus(int id)
        {
            QStatus qStatus = db.QStatus.Find(id);
            if (qStatus == null)
            {
                return NotFound();
            }

            db.QStatus.Remove(qStatus);
            db.SaveChanges();

            return Ok(qStatus);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QStatusExists(int id)
        {
            return db.QStatus.Count(e => e.QStatusID == id) > 0;
        }
    }
}