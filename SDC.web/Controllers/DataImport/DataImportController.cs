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
using System.Security.Cryptography;
using System.ComponentModel;
using System.Net;

namespace SDC.web.Controllers.DataImport
{
    public class DataImportController : AsyncController
    {
        [Authorize]
        // GET: DataImport
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Import(DataImportViewModel vm)
        {
            if(String.IsNullOrEmpty(vm.ImportPassword))
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

            string requiredHash = "aa7f5180ab596290cf8f1da45378a488";
            var md5 = MD5.Create();
            var bytes = md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(vm.ImportPassword));
            if (!requiredHash.Equals(BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            else
            {
                SDC.Library.DummyDataImport.ImportUtility import = new Library.DummyDataImport.ImportUtility();
                //load data.
                import.LoadData();
                //start import
                import.Import(max: vm.Max);

                return View();
            }
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