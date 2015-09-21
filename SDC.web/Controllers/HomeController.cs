using SDC.Library.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDC.web.Controllers
{
    public class HomeController : SDCController
    {
        // GET: Home
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var activeUsers = ActivityTracker.GetActiveUsers();
                return View(activeUsers);
            }
            else
                return RedirectToAction("Login", "Account");
        }
    }
}