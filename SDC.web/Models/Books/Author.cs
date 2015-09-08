using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.Models.Books
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public bool IsVerified { get; set; }
        public UserProfile AddedBy { get; set; }
        public ICollection<Book> Books { get; set; }
        //link to wikipedia url
        public string Url { get; set; }
    }
}