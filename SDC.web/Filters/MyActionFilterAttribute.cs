using SDC.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;

namespace SDC.web.Filters
{
    public class MyActionFilterAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// on every's request OnAuthorization: 
        /// get the current user's profile
        /// if not set already, set it as session data.
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                Library.Redis.ActivityTracker.TrackActive(filterContext.RequestContext.HttpContext.User.Identity.Name);

                if(filterContext.RequestContext.HttpContext.Session["UserInfo"] == null)
                {
                    using (var db = new SDCContext())
                    {
                        var profile = db.UserProfiles
                            .Include(p=>p.Avatar)
                            .Include(p=>p.Country.Language)
                            .FirstOrDefault(p => p.UserName == filterContext.RequestContext.HttpContext.User.Identity.Name);

                        if(profile != null)
                        {
                            profile.Role = Roles.GetRolesForUser(profile.UserName)[0];
                            profile.Shelves = db.Shelves.Where(p => p.Owner.UserId == profile.UserId).ToList();
                            filterContext.RequestContext.HttpContext.Session["UserInfo"] = profile;
                            filterContext.RequestContext.HttpContext.Session["UserInfoEx"] = profile.GetExtendedInfo(db);
                        }
                        else
                        {
                            filterContext.RequestContext.HttpContext.Session["UserInfo"] = null;
                            filterContext.RequestContext.HttpContext.Session["UserInfoEx"] = null;
                        }
                    }
                }
            }
        }
    }
}