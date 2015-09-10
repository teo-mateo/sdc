﻿using SDC.web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using System.IO;
using WebMatrix.WebData;
using Microsoft.AspNet.Identity;
using SDC.data;
using SDC.data.Entity;
using SDC.data.Entity.Profile;

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

                var customAvatar = (from av in db.Avatars
                                    where av.CustomForUserId == profile.UserId
                                    select av)
                                    .FirstOrDefault();

                if (customAvatar != null)
                    profilevm.CustomAvatar = customAvatar;

                profilevm.DefaultAvatars = db.Avatars.Where(p => p.CustomForUserId == 0).ToList();
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


            ((UserProfile)Session["UserInfo"]).Avatar = avatar;

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
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/");

            return View(model);
        }

        [HttpPost]
        [ActionName("ChangePassword")]
        public ActionResult ChangePassword_Post(ChangePasswordModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/");

            //if all fields are empty, just go back. 

            if (String.IsNullOrEmpty(model.OldPassword) ||
                String.IsNullOrEmpty(model.NewPassword) ||
                String.IsNullOrEmpty(model.NewPasswordRepeat))
            {
                return RedirectToAction("ChangePassword",
                    new { userMessage = "Please input the old password and the new passwod, twice." });
            }

            if (model.NewPassword != model.NewPasswordRepeat)
            {
                return RedirectToAction("ChangePassword",
                    new { userMessage = "Please input the old password and the new passwod, twice." });
            }

            if (model.OldPassword == model.NewPasswordRepeat)
            {
                return RedirectToAction("ChangePassword",
                    new { userMessage = "Please input the old password and the new passwod, twice." });
            }

            try
            {
                MembershipUser u = Membership.GetUser(User.Identity.Name);
                if (u.ChangePassword(model.OldPassword, model.NewPassword))
                {

                }
                else
                {
                    return RedirectToAction("ChangePassword",
                        new { userMessage = "Could not change password. Please try again or contact an administrator." });
                }
            }
            catch (Exception ex)
            {
                //todo: logging

                return RedirectToAction("ChangePassword",
                    new { userMessage = "Could not change password. Please try again or contact an administrator." });
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ChangeEmail(ChangeEmailModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/");

            var profile = db.UserProfiles.First(p => p.UserName == User.Identity.Name);
            model.Email = profile.Email;

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteAccount(UserProfileViewModel model)
        {
            try
            {
                if (Membership.ValidateUser(User.Identity.Name, model.Password))
                {
                    //delete profile and log out.
                    using (var db = new SDCContext())
                    using (var t = db.Database.BeginTransaction())
                    {
                        var profile = db.UserProfiles.First(p => p.UserName == User.Identity.Name);


                        //delete login traces for this account
                        var login_traces = db.LogInTraces.Where(p => p.User.UserId == profile.UserId).ToList();
                        db.LogInTraces.RemoveRange(login_traces);
                        //delete custom avatar
                        var custom_avatar = db.Avatars.FirstOrDefault(p => p.CustomForUserId == profile.UserId);
                        if (custom_avatar != null)
                        {
                            var relative_avatar_path = VirtualPathUtility.ToAppRelative(custom_avatar.Url);
                            var path = Server.MapPath(relative_avatar_path);
                            System.IO.File.Delete(path);
                            db.Avatars.Remove(custom_avatar);
                        }
                            

                        db.SaveChanges();
                        t.Commit();


                    }

                    //delete user profile
                    // I wonder if the transaction has anything to do with it... 
                    Membership.DeleteUser(User.Identity.Name, true);
                    WebSecurity.Logout();
                }
                else
                {
                    model.Message = "Enter your password to delete your account.";
                    //redirect to /profile/index#privacy
                    return Redirect(Url.RouteUrl(new
                    {
                        controller = "Profile",
                        action = "Index"
                    }) + "#DeleteProfile");
                }
            }
            catch (Exception ex)
            {
                //todo: log this shit.
            }

            return Redirect("/");
        }

        [HttpPost]
        [ActionName("ChangeEmail")]
        public ActionResult ChangeEmail_Post(ChangeEmailModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/");

            if (String.IsNullOrEmpty(model.Password))
            {
                return RedirectToAction("ChangeEmail",
                    new { userMessage = "Please enter your password to make this change." });
            }

            if (String.IsNullOrEmpty(model.Email))
            {
                return RedirectToAction("ChangeEmail",
                    new { userMessage = "Please enter the e-mail you wish to use." });
            }

            //check password.
            MembershipUser u = Membership.GetUser(User.Identity.Name);

            if (!Membership.ValidateUser(User.Identity.Name, model.Password))
            {
                return RedirectToAction("ChangeEmail",
                    new { userMessage = "Please enter your password to make this change." });
            }

            var profile = db.UserProfiles.First(p => p.UserName == User.Identity.Name);
            profile.Email = model.Email;
            db.SaveChanges();

            return RedirectToAction("ChangeEmail");
        }

        [HttpPost]
        public ActionResult UploadAvatar(UserProfileViewModel model)
        {
            if (model.ImageUpload != null && model.ImageUpload.ContentLength > 0)
            {
                var uploadDir = "~/Content/Uploads";
                var imagePath = Path.Combine(Server.MapPath(uploadDir), model.ImageUpload.FileName);
                var imageUrl = Path.Combine(uploadDir, model.ImageUpload.FileName);
                imageUrl = VirtualPathUtility.ToAbsolute(imageUrl);

                

                var profile = db.UserProfiles.First(p => p.UserName == User.Identity.Name);

                //now save the avatar entity
                //if there is already a custom avatar set up:
                // - delete old image
                // - use the new one.
                var customExisting = db.Avatars.FirstOrDefault(p => p.CustomForUserId == profile.UserId);
                if (customExisting != null)
                {
                    var path = Server.MapPath(customExisting.Url);
                    System.IO.File.Delete(path);

                    model.ImageUpload.SaveAs(imagePath);

                    customExisting.Url = imageUrl;
                    profile.Avatar = customExisting;
                }
                else
                {
                    model.ImageUpload.SaveAs(imagePath);

                    Avatar custom = new Avatar()
                    {
                        CustomForUserId = profile.UserId,
                        Url = imageUrl
                    };
                    db.Avatars.Add(custom);
                    profile.Avatar = custom;
                }

                db.SaveChanges();

                ((UserProfile)Session["UserInfo"]).Avatar = profile.Avatar;

            }

            return RedirectToAction("Index");
        }

    }
}