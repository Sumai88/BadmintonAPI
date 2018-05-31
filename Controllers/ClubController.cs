using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using BadmintonSvc.Models;

namespace BadmintonSvc.Controllers
{
    public class ClubController : ApiController
    {
        private BadmintonSvcContext db = new BadmintonSvcContext();

        // GET: api/Club
        public IQueryable<Club> GetClubs()
        {
            return db.Clubs;
        }

        // GET: api/Club/5
        [ResponseType(typeof(Club))]
        public IHttpActionResult GetClub(int id)
        {
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return NotFound();
            }

            return Ok(club);
        }

        [Route("api/Club/Info")]
        [ResponseType(typeof(Club))]
        public IHttpActionResult GetClubInfo(String strEmail)
        {
            Club club = db.Clubs.Where(p => p.ClubEmail.ToLower() == strEmail.ToLower()).First();
            if (club == null)
            {
                return NotFound();
            }
            return Ok(club);
        }

        // PUT: api/Club/5
        [ResponseType(typeof(Club))]
        public IHttpActionResult PutClub(int id, Club club)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != club.ClubID)
            {
                return BadRequest();
            }

            db.Entry(club).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClubExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(club);
        }

        // POST: api/Club
        [ResponseType(typeof(Club))]
        public IHttpActionResult PostClub(Club club)
        {
            int noOfCourts;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            noOfCourts = club.NoOfCourts;
            club.Status = 0;
            club.Created = DateTime.Now;
            db.Clubs.Add(club);
            db.SaveChanges();

            for (int i = 0; i<= noOfCourts; i++)
            {
                Court court = new Court();
                court.ClubID = club.ClubID;
                court.Status = 0;
                court.CourtNum = i;
                court.club = club;
                db.Courts.Add(court);
                db.SaveChanges();
            }

            return CreatedAtRoute("DefaultApi", new { id = club.ClubID }, club);
        }

        // DELETE: api/Club/5
        [ResponseType(typeof(Club))]
        public IHttpActionResult DeleteClub(int id)
        {
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return NotFound();
            }

            db.Clubs.Remove(club);
            db.SaveChanges();

            return Ok(club);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClubExists(int id)
        {
            return db.Clubs.Count(e => e.ClubID == id) > 0;
        }
    }
}