using SDC.data.Entity.Profile;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace SDC.data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SDCContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(SDCContext context)
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
            context.Avatars.AddOrUpdate(new Avatar() { Id = 1, Url = "~/Content/dist/img/avatar.png", CustomForUserId = 0 });
            context.Avatars.AddOrUpdate(new Avatar() { Id = 2, Url = "~/Content/dist/img/avatar2.png", CustomForUserId = 0 });
            context.Avatars.AddOrUpdate(new Avatar() { Id = 3, Url = "~/Content/dist/img/avatar3.png", CustomForUserId = 0 });
            context.Avatars.AddOrUpdate(new Avatar() { Id = 4, Url = "~/Content/dist/img/avatar4.png", CustomForUserId = 0 });
            context.Avatars.AddOrUpdate(new Avatar() { Id = 5, Url = "~/Content/dist/img/avatar5.png", CustomForUserId = 0 });
            #endregion

            #region Some (few) Cities in Romania
            context.Cities.AddOrUpdate(new data.Entity.Location.City() { Id = 1, Name = "București" });
            context.Cities.AddOrUpdate(new data.Entity.Location.City() { Id = 2, Name = "Craiova" });
            context.Cities.AddOrUpdate(new data.Entity.Location.City() { Id = 3, Name = "Ploiești" });
            #endregion

            #region Genres
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 1, Name = "Science fiction" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 2, Name = "Satire" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 3, Name = "Drama" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 4, Name = "Action and adventure" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 5, Name = "Romance" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 6, Name = "Mystery" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 7, Name = "Horror" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 8, Name = "Self help" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 9, Name = "Guide" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 10, Name = "Travel" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 11, Name = "Children's" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 12, Name = "Religious" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 13, Name = "Science" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 14, Name = "History" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 15, Name = "Math" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 16, Name = "Anthologies" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 17, Name = "Poetry" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 18, Name = "Encyclopedias" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 19, Name = "Dictionaries" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 20, Name = "Comics" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 21, Name = "Art" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 22, Name = "Cookbooks" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 23, Name = "Diaries" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 24, Name = "Journals" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 25, Name = "Prayer books" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 26, Name = "Series" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 27, Name = "Trilogies" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 28, Name = "Biographies" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 29, Name = "Autobiographies" });
            context.Genres.AddOrUpdate(new data.Entity.Books.Genre() { Id = 30, Name = "Fantasy" });
            #endregion

            #region some authors
            context.Authors.AddOrUpdate(new data.Entity.Books.Author() { Id = 1, Name = "Douglas Adams", IsVerified=true});
            context.Authors.AddOrUpdate(new data.Entity.Books.Author() { Id = 2, Name = "Isaac Asimov", IsVerified = true });
            context.Authors.AddOrUpdate(new data.Entity.Books.Author() { Id = 3, Name = "Tom Clancy", IsVerified = true });
            context.Authors.AddOrUpdate(new data.Entity.Books.Author() { Id = 4, Name = "Bill Bryson", IsVerified = true });
            context.Authors.AddOrUpdate(new data.Entity.Books.Author() { Id = 5, Name = "Mikhail Bulgakov", IsVerified = true });
            context.Authors.AddOrUpdate(new data.Entity.Books.Author() { Id = 6, Name = "Agatha Christie", IsVerified = true });
            context.Authors.AddOrUpdate(new data.Entity.Books.Author() { Id = 7, Name = "Albert Camus", IsVerified = true });
            context.Authors.AddOrUpdate(new data.Entity.Books.Author() { Id = 8, Name = "Aldous Huxley", IsVerified = true });
            context.Authors.AddOrUpdate(new data.Entity.Books.Author() { Id = 9, Name = "Alexander Pushkin", IsVerified = true });
            context.Authors.AddOrUpdate(new data.Entity.Books.Author() { Id = 10, Name = "Alexandre Dumas", IsVerified = true });
            context.Authors.AddOrUpdate(new data.Entity.Books.Author() { Id = 11, Name = "Anne Frank", IsVerified = true });

            #endregion

            #region Languages
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Code = "AF", Name = "Afrikaans" });
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Albanian", Code = "SQ"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Arabic", Code = "AR"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Armenian", Code = "HY"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Basque", Code = "EU"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Bengali", Code = "BN"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Bulgarian", Code = "BG"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Catalan", Code = "CA"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Cambodian", Code = "KM"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Chinese(Mandarin)", Code = "ZH"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Croatian", Code = "HR"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Czech", Code = "CS"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Danish", Code = "DA"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Dutch", Code = "NL"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "English", Code = "EN"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Estonian", Code = "ET"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Fiji", Code = "FJ"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Finnish", Code = "FI"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "French", Code = "FR"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Georgian", Code = "KA"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "German", Code = "DE"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Greek", Code = "EL"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Gujarati", Code = "GU"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Hebrew", Code = "HE"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Hindi", Code = "HI"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Hungarian", Code = "HU"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Icelandic", Code = "IS"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Indonesian", Code = "ID"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Irish", Code = "GA"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Italian", Code = "IT"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Japanese", Code = "JA"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Javanese", Code = "JW"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Korean", Code = "KO"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Latin", Code = "LA"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Latvian", Code = "LV"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Lithuanian", Code = "LT"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Macedonian", Code = "MK"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Malay", Code = "MS"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Malayalam", Code = "ML"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Maltese", Code = "MT"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Maori", Code = "MI"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Marathi", Code = "MR"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Mongolian", Code = "MN"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Nepali", Code = "NE"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Norwegian", Code = "NO"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Persian", Code = "FA"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Polish", Code = "PL"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Portuguese", Code = "PT"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Punjabi", Code = "PA"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Quechua", Code = "QU"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Romanian", Code = "RO"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Russian", Code = "RU"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Samoan", Code = "SM"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Serbian", Code = "SR"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Slovak", Code = "SK"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Slovenian", Code = "SL"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Spanish", Code = "ES"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Swahili", Code = "SW"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Swedish", Code = "SV"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Tamil", Code = "TA"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Tatar", Code = "TT"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Telugu", Code = "TE"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Thai", Code = "TH"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Tibetan", Code = "BO"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Tonga", Code = "TO"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Turkish", Code = "TR"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Ukrainian", Code = "UK"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Urdu", Code = "UR"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Uzbek", Code = "UZ"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Vietnamese", Code = "VI"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Welsh", Code = "CY"});
            context.Languages.AddOrUpdate(new data.Entity.Location.Language() { Name = "Xhosa", Code = "XH"});
            #endregion

        }


    }
}
