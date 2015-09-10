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


        public static string GetConnectionString()
        {
            try
            {
                var azureKey = "SQLAZURECONNSTR_sdcContext";
                var connectionString = Environment.GetEnvironmentVariable(azureKey);

                if (connectionString == null)
                {
                    var cs = System.Configuration.ConfigurationManager.ConnectionStrings["SDCConnectionString"];
                    if (cs == null)
                        return "sdc_no_connection_string";
                    else
                        return cs.ConnectionString;
                }

                return connectionString;
            }
            catch
            {
                return "sdc_no_connection_string";
            }
        }
    }
}
