using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BadmintonSvc.Models
{
    public class Process
    {
        public int ProcessID { get; set; }
        public String ProcessName { get; set; }
        public int Status { get; set; }
    }
}