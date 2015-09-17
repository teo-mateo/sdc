using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.data.Entity.Books
{
    public class BookPicture : IEntity
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public bool IsMain { get; set; }
    }
}