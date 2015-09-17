using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SDC.data.Entity.Books
{
    public class Genre : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }

        public static Genre[] GetAll(SDCContext db)
        {
            return (from g in db.Genres
                    orderby g.Name
                    select g).ToArray();
        }
    }
}