using System;
namespace BadmintonSvc.Models
{
    public class Logger
    {
        public int LoggerID { get; set; }
        public String LogMessage { get; set; }
        public String LogType { get; set; }
        public String LogSource { get; set; }
        private DateTime _logDateTime = DateTime.Now;
        public DateTime LogDate
        {
            get { return _logDateTime; }
            set { _logDateTime = value; }
        }
        public int? ClubID { get; set; }
        public int? PlayerID { get; set; }
        public int? QueueID { get; set; }
        public int? QueueTempID { get; set; }

    }
}