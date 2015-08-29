using SDC.web.Models.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.ViewModels
{
    public class ShelvesViewModel
    {
        public List<ShelfViewModel> Shelves { get; set; }
        public string NewShelfName { get; set; }
        public int DeleteShelfId { get; set; }
    }
}