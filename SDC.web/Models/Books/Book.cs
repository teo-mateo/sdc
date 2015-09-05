using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SDC.web.Models.Books
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
        public virtual ICollection<Author> Authors { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Genre> Genres { get; set; }

        public int Year { get; set; }
        public string Language { get; set; }
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