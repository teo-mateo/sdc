using log4net;
using Ninject;
using SDC.Library.NinjectModules;
using SDC.web.AutoMapperConfig;
using SDC.web.Filters;
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

    public class SDCApp : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            WebSecurity.InitializeDatabaseConnection("SDCConnectionString", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalFilters.Filters.Add(new SDCAuthorizationFilterAttribute());

            MappingsConfig.RegisterMappings();
            SDCApp.Kernel = new StandardKernel(new SDCModule());
        }

        public static IKernel Kernel
        {
            get; private set;
        }
    }
}
