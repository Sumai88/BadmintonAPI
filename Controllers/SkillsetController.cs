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
    public class SkillsetController : ApiController
    {
        private BadmintonSvcContext db = new BadmintonSvcContext();

        // GET: api/Skillset
        public IQueryable<Skillset> GetSkillsets()
        {
            return db.Skillsets;
        }

        // GET: api/Skillset/5
        [ResponseType(typeof(Skillset))]
        public IHttpActionResult GetSkillset(int id)
        {
            Skillset skillset = db.Skillsets.Find(id);
            if (skillset == null)
            {
                return NotFound();
            }

            return Ok(skillset);
        }

        // PUT: api/Skillset/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSkillset(int id, Skillset skillset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != skillset.SkillsetID)
            {
                return BadRequest();
            }

            db.Entry(skillset).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillsetExists(id))
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

        // POST: api/Skillset
        [ResponseType(typeof(Skillset))]
        public IHttpActionResult PostSkillset(Skillset skillset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Skillsets.Add(skillset);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = skillset.SkillsetID }, skillset);
        }

        // DELETE: api/Skillset/5
        [ResponseType(typeof(Skillset))]
        public IHttpActionResult DeleteSkillset(int id)
        {
            Skillset skillset = db.Skillsets.Find(id);
            if (skillset == null)
            {
                return NotFound();
            }

            db.Skillsets.Remove(skillset);
            db.SaveChanges();

            return Ok(skillset);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SkillsetExists(int id)
        {
            return db.Skillsets.Count(e => e.SkillsetID == id) > 0;
        }
    }
}