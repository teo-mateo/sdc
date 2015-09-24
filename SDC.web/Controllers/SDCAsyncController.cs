using SDC.Library.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDC.web.Controllers
{
    public class SDCAsyncController : AsyncController
    {
        private ControllerActionLogger _logger;

        public SDCAsyncController() : base()
        {
            _logger = new ControllerActionLogger();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _logger.StartAction(filterContext.ActionDescriptor.UniqueId);
            base.OnActionExecuting(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _logger.EndAction(
                filterContext.ActionDescriptor.UniqueId,
                filterContext.ActionDescriptor.ActionName,
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.HttpContext.Response.StatusCode.ToString());
            base.OnActionExecuted(filterContext);
        }
    }
}