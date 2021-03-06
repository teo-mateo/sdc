﻿using SDC.data.Entity.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.data.ViewModels
{
    public class ShelvesViewModel
    {
        public ShelfViewModel[] Shelves { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public int EditShelfId { get; set; }
        public int DeleteShelfId { get; set; }
        public string Message { get; set; }
    }
}