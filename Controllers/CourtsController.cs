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
    public class CourtsController : ApiController
    {
        private BadmintonSvcContext db = new BadmintonSvcContext();

        // GET: api/Courts
        public IQueryable<Court> GetCourts()
        {
            return db.Courts;
        }

        // GET: api/Courts/5
        [ResponseType(typeof(Court))]
        public IHttpActionResult GetCourt(int id)
        {
            Court court = db.Courts.Find(id);
            if (court == null)
            {
                return NotFound();
            }

            return Ok(court);
        }

        // PUT: api/Courts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCourt(int id, Court court)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != court.CourtID)
            {
                return BadRequest();
            }

            db.Entry(court).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourtExists(id))
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

        // POST: api/Courts
        [ResponseType(typeof(Court))]
        public IHttpActionResult PostCourt(Court court)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Courts.Add(court);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = court.CourtID }, court);
        }

        // DELETE: api/Courts/5
        [ResponseType(typeof(Court))]
        public IHttpActionResult DeleteCourt(int id)
        {
            Court court = db.Courts.Find(id);
            if (court == null)
            {
                return NotFound();
            }

            db.Courts.Remove(court);
            db.SaveChanges();

            return Ok(court);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CourtExists(int id)
        {
            return db.Courts.Count(e => e.CourtID == id) > 0;
        }
    }
}