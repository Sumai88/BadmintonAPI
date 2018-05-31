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
    public class PlayerController : ApiController
    {
        private BadmintonSvcContext db = new BadmintonSvcContext();

        // GET: api/Player
        public IQueryable<Player> GetPlayers()
        {
            return db.Players;
        }

        // GET: api/Player/5
        [ResponseType(typeof(Player))]
        public IHttpActionResult GetPlayer(int id)
        {
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return NotFound();
            }

            return Ok(player);
        }

        // GET: api/Player
        [Route("api/Player/Info")]
        [ResponseType(typeof(Player))]
        public IHttpActionResult GetPlayerInfo(String strEmail)
        {
            Player player = db.Players.Where(p=> p.PlayerEmail.ToLower() == strEmail.ToLower()).First();
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }

        // PUT: api/Player/5
        [ResponseType(typeof(Player))]
        public IHttpActionResult PutPlayer(int id, Player player)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != player.PlayerID)
            {
                return BadRequest();
            }

            db.Entry(player).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(player);
        }

        // POST: api/Player
        [ResponseType(typeof(Player))]
        public IHttpActionResult PostPlayer(Player player)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Player playerExists = db.Players.Where(p => p.PlayerEmail.ToLower() == player.PlayerEmail.ToLower()).FirstOrDefault();
            if (playerExists == null)
            {
                player.Created = DateTime.Now;
                player.DebugMode = false;
                db.Players.Add(player);
                db.SaveChanges();
                return CreatedAtRoute("DefaultApi", new { id = player.PlayerID }, player);
            }
            else
            {
                playerExists.PlayerName = player.PlayerName;
                playerExists.Password = player.Password;
                playerExists.Phone = player.Phone;
                playerExists.Created = DateTime.Now;
                db.Entry(playerExists).State = EntityState.Modified;
                db.SaveChanges();
                return CreatedAtRoute("DefaultApi", new { id = playerExists.PlayerID }, playerExists);
            }
            
        }

        // DELETE: api/Player/5
        [ResponseType(typeof(Player))]
        public IHttpActionResult DeletePlayer(int id)
        {
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return NotFound();
            }

            db.Players.Remove(player);
            db.SaveChanges();

            return Ok(player);
        }

        [Route("api/Login")]
        public int Login(String playerEmail, String strPassword)
        {
            Player player = db.Players.Where(p => p.PlayerEmail.ToLower() == playerEmail.ToLower()).Single();
            if (player == null)
                return 0;
            else if (player.PlayerEmail.Equals(playerEmail) && player.Password.Equals(strPassword))
                return player.PlayerID;
            return 0;
        }

        [Route("api/LoginDirect")]
        [ResponseType(typeof(Player))]
        public IHttpActionResult PostLoginDirect(String email, String pwd)
        {
            Player player = db.Players.Where(p => p.PlayerEmail.ToLower() == email.ToLower()).Single();
            if (player == null)
                return NotFound(); 
            else if (player.PlayerEmail.ToLower().Equals(email.ToLower()) && player.Password.Equals(pwd))
                return Ok(player);
            else
            {
                player.Password = "Incorrect Password";
                return Ok(player);
            }
        }

        [Route("api/LoginFromFB")]
        [ResponseType(typeof(Player))]
        public IHttpActionResult PostLoginFromFB(String email, String user, String name)
        {
            Player player = db.Players.Where(p => p.PlayerEmail.ToLower() == email.ToLower()).FirstOrDefault();
            if (player == null)
            {
                Player p = new Player();
                p.PlayerName = name;
                p.PlayerEmail = email;
                p.Username = user;
                p.LoginType = "Facebook";
                p.Created = DateTime.Now;
                p.DebugMode = false;
                // have to include update for phone and device id later
                db.Players.Add(p);
                db.SaveChanges();
                return Ok(p);
            }
            else
                return Ok(player);           
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlayerExists(int id)
        {
            return db.Players.Count(e => e.PlayerID == id) > 0;
        }
    }
}