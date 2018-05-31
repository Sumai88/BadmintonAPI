using System;

namespace BadmintonSvc.Models
{
    public class Court
    {
        public int CourtID { get; set; }
        public int CourtNum { get; set; }
        public int ClubID { get; set; }
        public Int32 Status { get; set; }
        public virtual Club club { get; set; }
    }
}