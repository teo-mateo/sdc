using SDC.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDC.web.Controllers
{
    public class HelperController : Controller
    {
        // GET: Helper
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetPublishersJson(string term = "")
        {
            using(var db = new SDCContext())
            {
                var publishers =
                    (from p in db.Publishers
                     orderby p.Name
                     where 
                        p.IsVerified && 
                        (String.IsNullOrEmpty(term) || p.Name.Contains(term))
                     select new
                     {
                         label = p.Name,
                         value = p.Id
                     }).ToArray();

                return Json(publishers, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetAuthorsJson(string term = "")
        {
            using(var db = new SDCContext())
            {
                var authors =
                    (from a in db.Authors
                     orderby a.Name
                     where 
                        a.IsVerified &&
                        (String.IsNullOrEmpty(term) || a.Name.Contains(term))
                     select new
                     {
                         label = a.Name,
                         value = a.Id
                     }).ToArray();

                return Json(authors, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetGenresJson(string term = "")
        {
            using (var db = new SDCContext())
            {
                var genres =
                    (from g in db.Genres
                     orderby g.Name
                     where (String.IsNullOrEmpty(term) || g.Name.Contains(term))
                     select new
                     {
                         label = g.Name,
                         value = g.Id
                     }).ToArray();

                return Json(genres, JsonRequestBehavior.AllowGet);
            }
        }
    }
}