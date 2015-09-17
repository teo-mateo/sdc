
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
        public void AttachProfile(UserProfile o)
        {
            if (Set<UserProfile>().Local.Any(local => o == local))
            {
                Entry<UserProfile>(o).State = EntityState.Unchanged;
            }
            else
            {
                Set<UserProfile>().Attach(o);
                Entry<UserProfile>(o).State = EntityState.Unchanged;
            }
        }

        public void Attach<T>(T o) where T : class, IEntity
        {
            if (Set<T>().Local.Any(local => o == local))
            {
                Entry<T>(o).State = EntityState.Unchanged;
            }
            else
            {
                Set<T>().Attach(o);
                Entry<T>(o).State = EntityState.Unchanged;
            }
        }

        public void AttachCodeEntity<T>(ref T o) where T :class, ICodeEntity
        {
            string code = o.Code;
            T local = Set<T>().Local.FirstOrDefault(l => l.Code.Equals(code));

            if (local != null)
            {
                o = local;
            }
            else
            {
                Set<T>().Attach(o);
                Entry<T>(o).State = EntityState.Unchanged;
            }
        }

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
            this.Database.Log = Console.WriteLine;
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<LogInTrace> LogInTraces { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Country> Countries { get; set; }
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