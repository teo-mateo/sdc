using SDC.web.Models;
using SDC.web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

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
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            else
            {
                var profile = db.UserProfiles
                    .Include("Avatar")
                    .Include("City")
                    .First(p => p.UserName == User.Identity.Name);

                UserProfileViewModel profilevm = new UserProfileViewModel();
                SDC.Library.Tools.CopySimpleProperties.Copy(profile, profilevm);
                SDC.Library.Tools.CopySimpleProperties.Copy(profile.Avatar, profilevm.Avatar);
                if(profile.City != null)
                    SDC.Library.Tools.CopySimpleProperties.Copy(profile.City, profilevm.City);
                profilevm.ComputedProperty = "some value";

                profilevm.DefaultAvatars = db.Avatars.Where(p=>p.CustomForUserId == 0).ToList();
                profilevm.AllCities = db.Cities.OrderBy(p => p.Name).ToList();
                return View(profilevm);
            }
            
        }

        public ActionResult ChangeAvatar(int avatarId)
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/");

            var profile = db.UserProfiles.First(p => p.UserName == User.Identity.Name);
            var avatar = db.Avatars.Find(avatarId);
            profile.Avatar = avatar;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ChangeCity(int cityId)
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/");

            var profile = db.UserProfiles.First(p => p.UserName == User.Identity.Name);
            var city = db.Cities.Find(cityId);
            profile.City = city;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SavePrivacyChanges(UserProfileViewModel upvm)
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/");

            var profile = db.UserProfiles.Find(upvm.UserId);
            profile.ShowEmail = upvm.ShowEmail;
            db.SaveChanges();

            //redirect to /profile/index#privacy
            return Redirect(Url.RouteUrl(new
            {
                controller="Profile", 
                action="Index"
            })+"#Privacy");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        public ActionResult ChangeEmail()
        {
            return View();
        }

    }
}