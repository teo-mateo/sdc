using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SDC.data;
using SDC.data.Entity;
using SDC.data.Entity.Profile;
using SDC.data.ViewModels;
using System.Net;
using SDC.Library.S3;

namespace SDC.web.Controllers
{
    public class AuthorsController : SDCController
    {
        // GET: Authors
        /// <summary>
        /// will display a list with all authors.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            UserProfile profile = ((UserProfile)Session["ProfileInfo"]);
            if (profile == null || profile.Role == RolesCustom.USER)
            {
                ViewBag.CanEdit = false;
            }
            else
            {
                ViewBag.CanEdit = true;
            }
            ViewBag.Breadcrumbs = Breadcrumb.Generate("Authors", "");

            return View();
        }

        /// <summary>
        /// shows details of an author
        /// </summary>
        /// <param name="id">id of author</param>
        /// <returns>view</returns>
        [HttpGet]
        public ActionResult ViewAuthor(int id)
        {

            using (var db = new SDCContext())
            {
                var author = db.Authors
                    .Include(a => a.AddedBy)
                    .Include(a => a.LastModifiedBy)
                    .Include(a => a.Books)
                    .First(a => a.Id == id);

                var model = AutoMapper.Mapper.Map<AuthorViewModel>(author);

                ViewBag.Breadcrumbs = Breadcrumb.Generate(
                    "Authors", Url.Action("Index", "Authors"),
                    author.Name, "");

                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddAuthor(AuthorViewModel authorViewModel)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateAuthor(AuthorViewModel authorViewModel)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteAuthor(int id)
        {
            try
            {
                var profile = (UserProfile)Session["UserInfo"];
                if (profile == null || profile.Role == RolesCustom.USER)
                    return RedirectToAction("Index", "Home");

                using(var db = new SDCContext())
                {
                    //delete books
                    //delete book images
                    //delete author

                    var author = db.Authors
                        .Include(a => a.Books)
                        .Include(a => a.Books.Select(b => b.Pictures))
                        .First(a => a.Id == id);

                    foreach(var b in author.Books)
                    {
                        foreach(var p in b.Pictures)
                        {
                            S3.DeleteFile(p.Key);
                            db.BookPictures.Remove(p);
                        }
                        b.Pictures.Clear();
                        db.Books.Remove(b);
                    }
                    db.Authors.Remove(author);
                    db.SaveChanges();
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult ApproveAuthor(int id)
        {
            try
            {
                var profile = (UserProfile)Session["UserInfo"];
                if (profile == null || profile.Role == RolesCustom.USER)
                    return RedirectToAction("Index", "Home");

                using (var db = new SDCContext())
                {
                    var author = db.Authors.Find(id);
                    author.IsVerified = true;
                    db.SaveChanges();
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetAllAuthorsJson(string term = "")
        {
            using (var db = new SDCContext())
            {
                bool filterOnlyWithBooks = false;
                bool.TryParse(this.Request.QueryString["onlyWithBooks"], out filterOnlyWithBooks);

                int recordsTotal = db.Authors.Count();

                var authors = db.Authors
                    .Where(p => !filterOnlyWithBooks || p.Books.Count > 0)
                    .OrderBy(p => p.Name)
                    .Select(a => new
                    {
                        id = a.Id.ToString(),
                        name = a.Name,
                        isverified = a.IsVerified.ToString(),
                        bookcount = a.Books.Count.ToString(),
                        addedby = (a.AddedBy != null) ? a.AddedBy.UserName : "-",
                        addeddate = a.AddedDate.Value
                    }).ToArray();

                var o = new
                {
                    draw = 2,
                    recordsTotal = recordsTotal,
                    recordsFiltered = authors.Length,
                    data = authors.Select(a => new string[] { a.id, a.name, a.isverified, a.bookcount, a.addedby, a.addeddate.ToString(G.DATE) }).ToArray()
                };

                return Json(o, JsonRequestBehavior.AllowGet);
            }
        }
    }
}