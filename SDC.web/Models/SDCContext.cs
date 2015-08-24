using SDC.web.Models.Audit;
using SDC.web.Models.Location;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SDC.web.Models
{
    public class SDCContext : DbContext
    {
        public SDCContext()
            : base(MvcApplication.GetConnectionString())
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<LogInTrace> LogInTraces { get; set; }
        public DbSet<Avatar> Avatars { get; set; }

        public DbSet<City> Cities { get; set; }
    }
}