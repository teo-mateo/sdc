using SDC.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
                if(filterContext.RequestContext.HttpContext.Session["UserInfo"] == null)
                {
                    using (var db = new SDCContext())
                    {
                        var userProfile = db.UserProfiles
                            .Include("Avatar")
                            .FirstOrDefault(p => p.UserName == filterContext.RequestContext.HttpContext.User.Identity.Name);

                        if(userProfile != null)
                        {
                            var role = Roles.GetRolesForUser(userProfile.UserName)[0];
                            userProfile.Role = role;
                            filterContext.RequestContext.HttpContext.Session["UserInfo"] = userProfile;
                        }
                        else
                        {
                            filterContext.RequestContext.HttpContext.Session["UserInfo"] = null;
                        }
                    }
                }
            }
        }
    }
}