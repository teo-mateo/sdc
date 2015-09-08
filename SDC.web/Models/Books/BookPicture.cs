using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.Models.Books
{
    public class BookPicture
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public bool IsMain { get; set; }
    }
}