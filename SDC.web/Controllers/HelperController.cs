using SDC.data.Entity.Books;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SDC.data;
using System.Collections;

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



        public ActionResult AuthorScrapeWiki()
        {
            string baseUrl = "https://en.wikipedia.org/wiki/List_of_authors_by_name:_";
            int updated = 0;


            //65..90
            using (var db = new SDCContext())
            {
                //load all authors
                db.Set<Author>().Load();

                for (int i = 65; i <= 90; i++)
                {
                    var url = baseUrl + (char)i;
                    Scrape(db, url, ref updated);
                }

                db.SaveChanges();
            }
            
            

            throw new NotImplementedException();

        }

        private void Scrape(SDCContext db, string url, ref int updated)
        {
            string token = "<ul>\n<li>";

            WebRequest req = WebRequest.Create(url);
            using (WebResponse response = req.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var sr = new StreamReader(stream))
            {
                var html = sr.ReadToEnd();

                int idx = html.IndexOf(token);
                while (idx > 0)
                {
                    if (html.Length < idx + 1)
                        break;

                    html = html.Substring(idx + 1);
                    int idx_end = html.IndexOf("</ul>");

                    if (idx_end < 0)
                        break;

                    string list_html = html.Substring(0, idx_end);
                    string[] lines = list_html.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries); // assuming \r\n. good enough for now.
                    lines = lines.Where(p => p.StartsWith("<li><a")).ToArray();

                    string title_attr = "title=\"";

                    foreach (var l in lines)
                    {
                        string line = l;
                        int auth_start = line.IndexOf(title_attr) + title_attr.Length;
                        line = line.Substring(auth_start);

                        string auth = line.Substring(0, line.IndexOf("\""));

                        //if none of the authors that were loaded into the local set has the same name as the one found, 
                        //then add it.
                        if (!db.Authors.Local.Any(p=>p.Name.ToLower().Equals(auth.ToLower())))
                        {
                            db.Authors.Local.Add(new Author()
                            {
                                Name = auth,
                                IsVerified = true
                            });
                        }
                        
                    }

                }
            }
        }




    }
}