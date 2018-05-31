using System;

namespace BadmintonSvc.Models
{
    public class QueueData
    {
        public int ClubID { get; set; }
        public DateTime PlayDateTime { get; set; }
        public int PlayerID { get; set; }
        public String PlayerName { get; set; }
        public int QStatusID { get; set; }
        public int QueueID { get; set; }
        public int Score { get; set; }
        public int SkillsetID { get; set; }
        public int QueueOrder { get; set; }
        public int QueueTempID { get; set; }
    }
}