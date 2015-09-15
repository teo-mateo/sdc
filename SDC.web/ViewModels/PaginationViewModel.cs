using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.ViewModels
{
    public interface IPaginationViewModel
    {
        PaginationViewModel Pagination
        {
            get;
        }
    }

    public class PaginationViewModel
    {
        public int Id { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string EntityName { get; set; }
        public int EntityCount { get; set; }

    }
}