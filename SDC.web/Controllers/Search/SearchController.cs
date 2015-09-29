using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using SDC.Library.ServiceLayer;
using SDC.Library.DTO;

namespace SDC.web.Controllers.Search
{
    public class SearchController : Controller
    {
        // GET: Search
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult BookSearch(string searchTerm, int searchId = 0, int page=1)
        {


            searchTerm = searchTerm == null ? "" : searchTerm.Trim();
            if (String.IsNullOrEmpty(searchTerm) || searchTerm.Length < 3)
            {
                ViewBag.Message = "search-error";
                return View(new SearchResultViewModel(SearchResultDTO.Empty()));
            }

            var service = SDCWebApp.Kernel.Get<ISDCService>();
            if (searchId == 0)
            {
                var searchResult = service.Search(searchTerm);
                return View(new SearchResultViewModel(searchResult));
            }
            else
            {
                var searchResult = service.SearchSubset(searchId, page-1, 10);
                if(searchResult.Id != -1)
                {
                    //possibly cache miss or?.. anything else.
                    return View(new SearchResultViewModel(searchResult));
                }
                else
                {
                    searchResult = service.Search(searchTerm);
                    return View(new SearchResultViewModel(searchResult));
                }
            }
        }
    }
}