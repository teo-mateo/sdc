using SDC.web.App_Start;
using SDC.web.Filters;
using SDC.web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebMatrix.WebData;

namespace SDC.web
{
    public class G
    {
        public static readonly string DATE = "MMM dd, yyyy";
    }

    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            WebSecurity.InitializeDatabaseConnection("SDCConnectionString", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalFilters.Filters.Add(new MyActionFilterAttribute());

            MappingsConfig.RegisterMappings();
        }



    }
}
