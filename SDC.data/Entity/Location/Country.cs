using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDC.data.Entity.Location
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

    public class Language
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Top { get; set; }

        public static Language[] GetAll(SDCContext db)
        {
            return db.Languages.OrderBy(p => p.Top).ThenBy(p => p.Name).ToArray();
        }
    }

}