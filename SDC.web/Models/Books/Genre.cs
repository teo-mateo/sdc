using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.Models.Books
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }

        internal static Genre[] GetAll(SDCContext db)
        {
            return (from g in db.Genres
                    orderby g.Name
                    select g).ToArray();
        }
    }
}