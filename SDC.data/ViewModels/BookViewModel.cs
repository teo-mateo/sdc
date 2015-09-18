using SDC.data.Entity.Books;
using SDC.data.Entity.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.data.ViewModels
{
    public class BookViewModel
    {
        public BookViewModel()
        {
            Authors = new List<Author>();
            Genres = new List<Genre>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string ISBN { get; set; }
        public List<Author> Authors { get; set; }
        public Publisher Publisher { get; set; }
        public List<Genre> Genres { get; set; }
        public List<BookPicture> Pictures { get; set; }
        public Language Language { get; set; }
        public int ShelfId { get; set; }
        public string Description { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }

        public DateTime AddedDate { get; set; }
        
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