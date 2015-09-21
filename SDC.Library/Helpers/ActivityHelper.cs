using SDC.data;
using SDC.data.Entity;
using SDC.data.Entity.Books;
using SDC.data.Entity.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Library.Helpers
{
    public static class ActivityHelper
    {
        public static void Activity_BookAdded(SDCContext db, UserProfile profile, Book book, string bookurl, string shelfurl)
        {
            string template = "<p>Added <a href = '%bookurl%'> <strong>%booktitle%</strong> </a> to <a href = '%shelfurl%'> <strong>%shelfname%</strong> </a> <span class='text-muted'>on %when%</span></p>";
            var content = template
                .Replace("%bookurl%", bookurl)
                .Replace("%booktitle%", book.Title)
                .Replace("%shelfurl%", shelfurl)
                .Replace("%shelfname%", book.Shelf.Name)
                .Replace("%when%", DateTime.Now.ToString(G.DATE));

            Activity activity = new Activity()
            {
                Profile = profile,
                Content = content, 
                Type = ActivityType.AddBook
            };

            db.Activities.Add(activity);
            db.SaveChanges();
        }

        public static void Activity_BookUpdated(SDCContext db, UserProfile profile, Book book, string bookurl, string shelfurl)
        {
            string template = "<p>Updated <a href = '%bookurl%'> <strong>%booktitle%</strong> </a> in <a href = '%shelfurl%'> <strong>%shelfname%</strong> </a> <span class='text-muted'>on %when%</span></p>";
            var content = template
                .Replace("%bookurl%", bookurl)
                .Replace("%booktitle%", book.Title)
                .Replace("%shelfurl%", shelfurl)
                .Replace("%shelfname%", book.Shelf.Name)
                .Replace("%when%", DateTime.Now.ToString(G.DATE));

            Activity activity = new Activity()
            {
                Profile = profile,
                Content = content,
                Type = ActivityType.UpdateBook
            };

            db.Activities.Add(activity);
            db.SaveChanges();
        }

        public static void Activity_BookRemoved(SDCContext db, UserProfile profile, Book book, string shelfName)
        {
            string template = "<p>Removed <strong>%booktitle% </strong> from %shelfname% <span class='text-muted'>on %when%</span></p>";
            var content = template
                .Replace("%booktitle%", book.Title)
                .Replace("%shelfname%", shelfName)
                .Replace("%when%", DateTime.Now.ToString(G.DATE));

            Activity activity = new Activity()
            {
                Profile = profile,
                Content = content,
                Type = ActivityType.RemoveBook
            };

            db.Activities.Add(activity);
            db.SaveChanges();
        }
    }
}
