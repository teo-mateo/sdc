using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using SDC.Library.NinjectModules;

namespace SDC.web.Controllers
{
    public class SDCController : Controller
    {
        public SDCController() : base()
        {

        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var log = log4net.LogManager.GetLogger(this.GetType());
            base.OnException(filterContext);
        }
    }
}