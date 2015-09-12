using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SDC.data.Entity.Location
{
    public class Country
    {
        [Key]
        public string Code { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public Language Language { get; set; }
        public bool IsVisible { get; set; }

        public static Country[] GetAll(SDCContext db, bool onlyVisible=true)
        {
            return db.Countries
                .Where(p=>p.IsVisible || !onlyVisible)
                .OrderBy(p => p.Name).ToArray();
        }
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Country Country { get; set; }
        public bool IsVisible { get; set; }

        public static City[] GetAll(SDCContext db, string countryCode, bool onlyVisible = true)
        {
            return db.Cities
                .Where(p => p.Country.Code == countryCode && (p.IsVisible || !onlyVisible))
                .OrderBy(p => p.Name).ToArray();
        }
    }

    public class Language
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Top { get; set; }
        public bool IsVisible { get; set; }

        public static Language[] GetAll(SDCContext db, bool onlyVisible=true)
        {
            return db.Languages
                .Where(l=>l.IsVisible || !onlyVisible)
                .OrderBy(p => p.Top).ThenBy(p => p.Name).ToArray();
        }
    }

}