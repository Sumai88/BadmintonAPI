using System;
using System.ComponentModel.DataAnnotations;

namespace BadmintonSvc.Models
{
    //[JsonObject(IsReference = true)]
    public class Club
    {
        public int ClubID { get; set; }
        public String ClubName { get; set; }
        public int NoOfCourts { get; set; }
        public String Organizer { get; set; }
        [EmailAddress]
        public String ClubEmail { get; set; }
        public String StreetName { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public Int64 Zipcode { get; set; }
        public Int32 Status { get; set; }
        private DateTime _created = DateTime.Now;
        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }
        public Boolean SkillPredefined { get; set; }
        //[JsonIgnore]
        //[IgnoreDataMember]
        //public virtual ICollection<Player> Players { get; set; }
    }
}   