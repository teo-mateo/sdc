using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SDC.web.Controllers
{
    public class PublicProfilesController : SDCController
    {
        // GET: PublicProfiles
        /// <summary>
        /// show a searchable directory of user profiles that are public?
        /// do I want to do that?
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                //will be picked up by the generic error handler
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            return View();
        }

        [HttpGet]
        public ActionResult View(string userName)
        {
            return View();
        }
    }
}