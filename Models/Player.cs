using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BadmintonSvc.Models
{
    //[JsonObject(IsReference = true)]        
    public class Player
    {
        public int PlayerID { get; set; }
        [Required]
        public String PlayerName { get; set; }
        [Required]
        [EmailAddress]
        public String PlayerEmail { get; set; }
        public long Phone { get; set; }
        public String Preference { get; set; } // Singles or Doubles
        public String Username { get; set; }
        public String Password { get; set; }
        public String LoginType { get; set; } //Social Media or direct signup
        public String DeviceID { get; set; } //for notifications
        public Int64 MixInd { get; set; }
        private DateTime _created = DateTime.Now;
        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }
        public int SkillsetID { get; set; }
        public bool DebugMode { get; set; }
        //public virtual Skillset Skillset { get; set; }
    }
}