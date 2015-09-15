using SDC.data.Entity.Books;
using SDC.data.Entity.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.ViewModels
{


    public class ShelfViewModel : IPaginationViewModel
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public int BookCount { get; set; }
        public bool IsVisible { get; set; }

        public Language DefaultLanguage { get; set; }

        public List<Book> Books { get; set; }
        public Language[] Languages { get; set; }
        public Genre[] Genres { get; set; }

        public PaginationViewModel Pagination { get; set; }
    }
}