using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult BookSearch(string searchTerm)
        {
            return View();
        }
    }
}