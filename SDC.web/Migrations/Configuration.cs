namespace SDC.web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SDC.web.Models.SDCContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SDC.web.Models.SDCContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            #region Default avatars
            context.Avatars.AddOrUpdate(new Models.Avatar() { Id = 1, Url = "/Content/dist/img/avatar.png", CustomForUserId = 0 });
            context.Avatars.AddOrUpdate(new Models.Avatar() { Id = 2, Url = "/Content/dist/img/avatar2.png", CustomForUserId = 0 });
            context.Avatars.AddOrUpdate(new Models.Avatar() { Id = 3, Url = "/Content/dist/img/avatar3.png", CustomForUserId = 0 });
            context.Avatars.AddOrUpdate(new Models.Avatar() { Id = 4, Url = "/Content/dist/img/avatar4.png", CustomForUserId = 0 });
            context.Avatars.AddOrUpdate(new Models.Avatar() { Id = 5, Url = "/Content/dist/img/avatar5.png", CustomForUserId = 0 });
            #endregion

            #region Some (few) Cities in Romania
            context.Cities.AddOrUpdate(new Models.Location.City() { Id = 1, Name = "București" });
            context.Cities.AddOrUpdate(new Models.Location.City() { Id = 2, Name = "Craiova" });
            context.Cities.AddOrUpdate(new Models.Location.City() { Id = 3, Name = "Ploiești" });
            #endregion
        }


    }
}
