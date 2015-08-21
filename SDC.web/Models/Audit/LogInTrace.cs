using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.Models.Audit
{
    public class LogInTrace
    {
        public int Id { get; set; }
        public UserProfile User { get; set; }
        public DateTime Timestamp { get; set; }
        public string IPAddress { get; set; }
    }
}