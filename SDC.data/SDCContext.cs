
using SDC.data.Entity;
using SDC.data.Entity.Audit;
using SDC.data.Entity.Books;
using SDC.data.Entity.Location;
using SDC.data.Entity.Profile;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SDC.data
{
    public class SDCContext : DbContext
    {
        public static string GetConnectionString()
        {
            try
            {
                var azureKey = "SQLAZURECONNSTR_sdcContext";
                var connectionString = Environment.GetEnvironmentVariable(azureKey);

                if (connectionString == null)
                {
                    var cs = System.Configuration.ConfigurationManager.ConnectionStrings["SDCConnectionString"];
                    if (cs == null)
                        return "sdc_no_connection_string";
                    else
                        return cs.ConnectionString;
                }

                return connectionString;
            }
            catch
            {
                return "sdc_no_connection_string";
            }
        }

        public SDCContext()
            : base(GetConnectionString())
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<LogInTrace> LogInTraces { get; set; }
        public DbSet<Avatar> Avatars { get; set; }

        public DbSet<City> Cities { get; set; }
        public DbSet<Language> Languages { get; set; }
        
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Book> Books { get; set; }

        public DbSet<Settings> Settings { get; set; }
    }
}