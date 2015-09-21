using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDC.web.Controllers
{
    public class AdministrationController : SDCController
    {
        // GET: Administration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View();
        }
    }
}