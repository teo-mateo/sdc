using Amazon.OpsWorks.Model;
using SDC.data.Entity.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.ViewModels
{
    //now I shall get my feet wet with .... AUTOMAPPER!
    //other properties will come at a later time. 
    // for now I can't think of any.

    public class AuthorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }

        public DateTime? Born { get; set; }
        public DateTime? Died { get; set; }

        public string Bio { get; set; }
        public bool IsVerified { get; set; }
        public string PictureUrl { get; set; }
        public string AddedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public ICollection<BookViewModel> Books { get; set; }
        //link to wikipedia url
        public string ExternalUrl { get; set; }
    }
}