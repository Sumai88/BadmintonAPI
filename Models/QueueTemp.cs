using System;

namespace BadmintonSvc.Models
{
    public class QueueTemp
    {
        public int QueueTempID { get; set; }
        public int QueueID { get; set; }
        public int ClubID { get; set; }
        public int PlayerID { get; set; }
        public int SkillsetID { get; set; } // Beginner, intermediate & Advanced
        public int QueueOrder { get; set; }
        public int QStatusID { get; set; }
        private DateTime _created = DateTime.Now;
        public DateTime Created {
            get { return _created; }
            set { _created = value; }
        }
        public virtual Queue Queue { get; set; }
    }
}   