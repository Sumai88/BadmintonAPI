using System;

namespace BadmintonSvc.Models
{
    public class Queue
    {
        public int QueueID { get; set; }
        public int ClubID { get; set; }
        public int PlayerID { get; set; }
        public int SkillsetID { get; set; } // Beginner, intermediate & Advanced
        public int Score { get; set; }
        private DateTime _playDateTime = DateTime.Now;
        public DateTime PlayDateTime
        {
            get { return _playDateTime; }
            set { _playDateTime = value; }
        }
        public int QStatusID { get; set; }
        public bool Won { get; set; }
        //public int GameID { get; set; }
        public virtual Club Club { get; set; }
        public virtual Player Player { get; set; }
        public virtual Skillset Skillset { get; set; }
        public virtual QStatus QStatus { get; set; }
    }
}