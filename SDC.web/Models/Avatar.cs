using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.Models
{
    public class Avatar 
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int CustomForUserId { get; set; }
    }
}