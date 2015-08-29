using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDC.web.Models.Books
{
    public class Shelf
    {
        public int Id { get; set; }
        public virtual UserProfile Owner { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        public bool IsVisible { get; set; }
    }
}