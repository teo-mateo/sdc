using SDC.web.Models.Books;
using SDC.web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.ViewModels
{
    public class ShelfViewModel
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public int BookCount { get; set; }
        public bool IsVisible { get; set; }

        public List<Book> Books { get; set; }
        public Language[] Languages { get; set; }
        public Genre[] Genres { get; set; }
    }
}