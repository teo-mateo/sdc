﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using SDC.Library.NinjectModules;
using System.Collections.Concurrent;
using ServiceStack.Redis;
using System.IO;
using Newtonsoft.Json;
using SDC.Library.Controllers;

namespace SDC.web.Controllers
{
    public class SDCController : Controller
    {
        private ControllerActionLogger _logger;

        public SDCController() : base()
        {
            _logger = new ControllerActionLogger();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _logger.StartAction(filterContext.ActionDescriptor.UniqueId);
            base.OnActionExecuting(filterContext);
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