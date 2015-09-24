using SDC.Library.DummyDataImport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ninject;
using ServiceStack.Redis;
using System.Diagnostics;

namespace SDC.web.Controllers.DataImport
{
    public class DataImportController : AsyncController
    {
        // GET: DataImport
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(DataImportViewModel vm)
        {
            SDC.Library.DummyDataImport.ImportUtility import = new Library.DummyDataImport.ImportUtility();
            //load data.
            import.LoadData();
            //start import
            import.Import(max:vm.Max);

            return View();
        }

        [HttpGet]
        public JsonResult Status()
        {
            var startedAt = ImportUtility.ImportStart;
            return Json(new
            {
                count = ImportUtility.Progress,
                target = ImportUtility.TargetCount,
                time = DateTime.Now.Subtract(startedAt).ToString(),
                running = ImportUtility.Running
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Cancel()
        {
            ImportUtility.Cancel = true;
            return Json(new { cancel = true }, JsonRequestBehavior.AllowGet);
        }
    }
}