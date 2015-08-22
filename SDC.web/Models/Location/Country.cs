using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.Models.Location
{
    [Obsolete]
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}