﻿using System;
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
        public virtual ICollection<Book> Books { get; set; }
    }
}