using SDC.data.Entity.Location;
using System;
using System.Collections.Generic;

namespace SDC.data.Entity.Books
{
    public class Book
    {
        public Book()
        {
            Authors = new List<Author>();
            Genres = new List<Genre>();
            Pictures = new List<BookPicture>();
        }

        public int Id { get; set; }
        public ICollection<Author> Authors { get; set; }
        public string Title { get; set; }
        public ICollection<Genre> Genres { get; set; }

        public int Year { get; set; }
        public Language Language { get; set; }
        public int? Pages { get; set; }
        public string ISBN { get; set; }
        public Publisher Publisher { get; set; }
        public string Description { get; set; }
        public virtual ICollection<BookPicture> Pictures { get; set; }

        //todo: currencies; link to user profile?
        public decimal? Price { get; set; }
        public DateTime AddedDate { get; set; }
        public Shelf Shelf { get; set; }
    }
}