using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.data.ViewModels
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
        /// <summary>
        /// generic id of the parent entity for the children of which we are showing the pagination
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// controller
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// Action to call to move to the next page
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// Current page
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// page size
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// total pages to display
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// Generic name of the entity
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// how many entities we are displaying on this page.
        /// </summary>
        public int EntityCount { get; set; }

    }
}