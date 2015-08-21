using SDC.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDC.web.Controllers
{
    /// <summary>
    /// this controller manages the user profile page from the perspective of the same user.
    /// .. global user management (and their profiles) will be done somewhere else.
    /// </summary>
    public class ProfileController : Controller
    {
        private SDCContext db = new SDCContext();

        // GET: Profile
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var profile = db.UserProfiles.First(p => p.UserName == User.Identity.Name);
            return View(profile);
        }
    }
}