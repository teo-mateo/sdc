using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.data.ViewModels
{
    public class AuthorListViewModel
    {
        public List<AuthorViewModel> Authors { get; set; }
        public int AuthorsCount { get; set; }
        public string SearchTerm { get; set; }
    }
}