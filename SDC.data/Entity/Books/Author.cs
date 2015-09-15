using System;
using System.Collections.Generic;

namespace SDC.data.Entity.Books
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }

        public DateTime? Born { get; set; }
        public DateTime? Died { get; set; }

        public string Bio { get; set; }
        public bool IsVerified { get; set; }
        public string PictureUrl { get; set; }
        public UserProfile AddedBy { get; set; }
        public UserProfile LastModifiedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public ICollection<Book> Books { get; set; }
        //link to wikipedia url
        public string ExternalUrl { get; set; }
    }


}