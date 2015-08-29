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
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
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

            #region Genres
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 1, Name = "Science fiction" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 2, Name = "Satire" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 3, Name = "Drama" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 4, Name = "Action and adventure" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 5, Name = "Romance" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 6, Name = "Mystery" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 7, Name = "Horror" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 8, Name = "Self help" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 9, Name = "Guide" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 10, Name = "Travel" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 11, Name = "Children's" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 12, Name = "Religious" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 13, Name = "Science" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 14, Name = "History" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 15, Name = "Math" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 16, Name = "Anthologies" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 17, Name = "Poetry" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 18, Name = "Encyclopedias" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 19, Name = "Dictionaries" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 20, Name = "Comics" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 21, Name = "Art" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 22, Name = "Cookbooks" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 23, Name = "Diaries" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 24, Name = "Journals" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 25, Name = "Prayer books" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 26, Name = "Series" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 27, Name = "Trilogies" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 28, Name = "Biographies" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 29, Name = "Autobiographies" });
            context.Genres.AddOrUpdate(new Models.Books.Genre() { Id = 30, Name = "Fantasy" });
            #endregion

            #region some authors
            context.Authors.AddOrUpdate(new Models.Books.Author() { Id = 1, Name = "Douglas Adams" });
            context.Authors.AddOrUpdate(new Models.Books.Author() { Id = 2, Name = "Isaac Asimov" });
            context.Authors.AddOrUpdate(new Models.Books.Author() { Id = 3, Name = "Tom Clancy" });
            context.Authors.AddOrUpdate(new Models.Books.Author() { Id = 4, Name = "Bill Bryson" });
            context.Authors.AddOrUpdate(new Models.Books.Author() { Id = 5, Name = "Mikhail Bulgakov" });
            #endregion

        }


    }
}
