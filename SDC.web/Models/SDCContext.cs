using SDC.web.Models.Audit;
using SDC.web.Models.Books;
using SDC.web.Models.Common;
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
        public DbSet<Language> Languages { get; set; }
        
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}