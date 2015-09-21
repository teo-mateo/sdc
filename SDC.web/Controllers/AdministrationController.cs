using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using Newtonsoft.Json;
using SDC.Library.Controllers;
using SDC.data.Entity;
using SDC.data.Entity.Profile;
using System.Net;

namespace SDC.web.Controllers
{
    public class AdministrationController : SDCController
    {
        // GET: Administration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View();
        }

        /// <summary>
        /// order by: controller-action-asc, controller-action-desc, hitcount-asc, hitcount-desc, avg-response-asc, avg-response-desc.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult AppMonitor(string orderby = "controller-action-asc")
        {
            //some 'security'
            var profile = (UserProfile)Session["UserInfo"];
            if(profile.Role != RolesCustom.ADMIN)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            ViewBag.OrderBy = orderby;

            //old log entries will be purged by another subsystem
            //so, getting ALL log items is <good enough> for now.

            IRedisClient cli = SDCWebApp.Kernel.Get<IRedisClient>();
            var set = cli.Sets["controller:log"];
            var all = set.GetAll()
                .Select(l => JsonConvert.DeserializeObject<ControllerActionLogItem>(l));

            LogViewModel logVM = new LogViewModel();

            foreach(var item in all)
            {
                string controllerActionPair = item.Controller + "/" + item.Action;
                if (logVM.ContainsPair(controllerActionPair))
                {
                    logVM.Get(controllerActionPair).Add(item);
                }
                else
                {
                    var logItem = new LogItem(controllerActionPair);
                    logItem.Add(item);
                    logVM.Add(logItem);
                }
            }

            logVM.ForEach(p => p.CalculateAverage());

            switch (orderby)
            {
                case ControllersOrderBy.ControllerActionAsc:
                    return View(logVM.OrderBy(p => p.ControllerAction));
                case ControllersOrderBy.ControllerActionDesc:
                    return View(logVM.OrderByDescending(p => p.ControllerAction));
                case ControllersOrderBy.HitCountAsc:
                    return View(logVM.OrderBy(p => p.Count));
                case ControllersOrderBy.HitCountDesc:
                    return View(logVM.OrderByDescending(p => p.Count));
                case ControllersOrderBy.AvgResponseAsc:
                    return View(logVM.OrderBy(p => p.Average));
                case ControllersOrderBy.AvgResponseDesc:
                    return View(logVM.OrderByDescending(p => p.Average));
            }


            return View(logVM);
        }
    }

    public class ControllersOrderBy
    {
        public const string ControllerActionAsc = "controller-action-asc";
        public const string ControllerActionDesc = "controller-action-desc";
        public const string HitCountAsc = "hitcount-asc";
        public const string HitCountDesc = "hitcount-desc";
        public const string AvgResponseAsc = "avgresponse-asc";
        public const string AvgResponseDesc = "avgresponse-desc";
    }

    public class LogViewModel : List<LogItem>
    {
        public bool ContainsPair(string controllerActionPair)
        {
            return this.Any(p => p.ControllerAction.Equals(controllerActionPair));
        }

        public LogItem Get(string controllerActionPair)
        {
            return this.First(p => p.ControllerAction.Equals(controllerActionPair));
        }

        public void CalculateAvg()
        {
            this.ForEach(p =>
            {
                p.CalculateAverage();
                p.Slowest = p.OrderByDescending(q => q.Duration).First();
            });
        }
    }

    public class LogItem : List<ControllerActionLogItem>
    {
        public LogItem(string controllerAction)
        {
            this.ControllerAction = controllerAction;
        }

        public string ControllerAction { get; set; }

        public int Average { get; private set; }
        public ControllerActionLogItem Slowest { get; set; }
        public void CalculateAverage()
        {
            this.Average = (int)this.Average(p => p.Duration);
        }
    }
}