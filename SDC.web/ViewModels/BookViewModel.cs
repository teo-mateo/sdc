using SDC.data.Entity.Books;
using SDC.data.Entity.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.ViewModels
{
    public class BookViewModel
    {
        public BookViewModel()
        {
            Authors = new List<Author>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string ISBN { get; set; }
        public List<Author> Authors { get; set; }
        public Publisher Publisher { get; set; }
        public List<Genre> Genres { get; set; }
        public Language Language { get; set; }
        public int ShelfId { get; set; }

    }

    //public class AuthorViewModel
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    //public class PublisherViewModel
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}