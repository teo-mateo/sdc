using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDC.web.Controllers
{
    public class DirectoryController : SDCController
    {
        // GET: Directory
        public ActionResult Index()
        {
            return View();
        }
    }
}