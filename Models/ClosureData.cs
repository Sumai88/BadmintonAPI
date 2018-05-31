using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BadmintonSvc.Models
{
    public class ClosureData
    {
        public int QueueID { get; set; }
        public int Score { get; set; }
        public bool Won { get; set; }
    }
}