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
using System.Diagnostics;
using SDC.data.Entity.Books;
using SDC.Library.Extensions;

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

            //these values may be missing from the query string.
            //these will be used by Datatables to "remember" the sorting 
            ViewBag.ColSortIndex = Request.QueryString["col"];
            ViewBag.ColSortDirection = Request.QueryString["ord"];

            return View();
        }

        /// <summary>
        /// shows details of an author
        /// </summary>
        /// <param name="id">id of author</param>
        /// <returns>view</returns>
        [HttpGet]
        public ActionResult ViewAuthor(int id, int page=1, int pagesize=0)
        {
            if (id == 0)
                return RedirectToAction("Index", "Home");

            var profile = (UserProfile)this.Session["UserInfo"];
            if (profile == null)
                return RedirectToAction("Index", "Home");

            if (pagesize < 1 || pagesize > 100)
                pagesize = profile.PageSize;

            using (var db = new SDCContext())
            {
                profile.UpdatePageSize(db, pagesize);

                var author = db.Authors
                    .Include(a => a.AddedBy)
                    .Include(a => a.LastModifiedBy)
                    .Include(a => a.Books)
                    .FirstOrDefault(a => a.Id == id);

                if (author == null)
                    return RedirectToAction("Index", "Home");

                int totalPages = ((int)Math.Ceiling((double)author.Books.Count / pagesize));
                if (page > totalPages)
                    page = totalPages;

                var model = AutoMapper.Mapper.Map<AuthorViewModel>(author);

                //actual pagination takes place here
                var show_books = author.Books
                        .OrderBy(b => b.AddedDate)
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .Select(b => AutoMapper.Mapper.Map<BookViewModel>(b));

                model.Pagination = new PaginationViewModel()
                {
                    Id = author.Id,
                    Action = "ViewAuthor",
                    Controller = "Authors",
                    Page = page,
                    PageSize = pagesize,
                    TotalPages = totalPages,
                    EntityCount = show_books.Count(),
                    EntityName = "Books"
                };

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

                using (var db = new SDCContext())
                using(var trans = db.Database.BeginTransaction())
                {
                    //delete books
                    //delete book images
                    //delete author

                    var books = db.Books
                        .Include(b => b.Pictures)
                        .Where(b => b.Authors.Any(a => a.Id == id)).ToArray();

                    foreach (var book in books)
                    {
                        //delete book images
                        foreach (var pic in book.Pictures.ToArray())
                        {
                            //delete from s3
                            if (!String.IsNullOrEmpty(pic.Key))
                            {
                                S3.DeleteFile(pic.Key);
                            }
                            //delete from db
                            db.BookPictures.Remove(pic);
                        }

                        //delete book
                        db.Books.Remove(book);
                    }

                    var author = db.Authors
                        .Include(a => a.Books)
                        .Include(a => a.Books.Select(b => b.Pictures))
                        .First(a => a.Id == id);

                    db.Authors.Remove(author);
                    db.SaveChanges();
                    trans.Commit();
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
                    author.LastModifiedBy = db.AttachProfile(profile);
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
        public JsonResult GetAllAuthorsJson()
        {
            using (var db = new SDCContext())
            {
                bool filterOnlyWithBooks = false;
                bool.TryParse(this.Request.QueryString["onlyWithBooks"], out filterOnlyWithBooks);

                int skip = Int32.Parse(this.Request.QueryString["start"]);
                int take = Int32.Parse(this.Request.QueryString["length"]);
                int draw = Int32.Parse(this.Request.QueryString["draw"]);

                string nameFilter = Request.QueryString["search[value]"];

                var allAuthorsQuery = db.Authors
                    .Where(p => (!filterOnlyWithBooks || p.Books.Count > 0) && (String.IsNullOrEmpty(nameFilter) || p.Name.Contains(nameFilter)))
                    .AsQueryable();
                
                string orderByField = TranslateColumnOrderBy(Request.QueryString["order[0][column]"]);
                string orderDirection = TranslateColumnOrderDirection(Request.QueryString["order[0][dir]"]);

                
                var orderedQuery = allAuthorsQuery.OrderByAnyDirection(orderByField, orderDirection);

                int filteredCount = orderedQuery.Count();

                var allAuthors = orderedQuery
                    .Skip(skip).Take(take)
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
                    draw = draw,
                    recordsTotal = filteredCount,
                    recordsFiltered = filteredCount,
                    data = allAuthors.Select(a => new string[] {
                        a.id,
                        a.name,
                        a.isverified,
                        a.bookcount,
                        a.addedby,
                        a.addeddate.ToString(Library.G.DATE)
                    }).ToArray()
                };

                return Json(o, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Translates the column index into the property name by wich the ordering will be done.
        /// </summary>
        /// <param name="colIndex">column index</param>
        /// <returns></returns>
        private string TranslateColumnOrderBy(string colIndex)
        {
            switch (colIndex)
            {
                case "0":
                    return "Id";
                case "1":
                    return "Name";
                case "2":
                    return "IsVerified";
                case "3":
                    return "Books.Count"; 
                case "4":
                    return "AddedBy.UserName";
                case "5":
                    return "AddedDate";
                default:
                    return "Id";
            }
        }

        /// <summary>
        /// Translates the ordering direction into strings more appropriate for Linq.
        /// </summary>
        /// <param name="direction">asc/desc</param>
        /// <returns>OrderBy/OrderByDescending</returns>
        private string TranslateColumnOrderDirection(string direction)
        {
            switch (direction)
            {
                case "asc":
                    return "OrderBy";
                case "desc":
                    return "OrderByDescending";
                default:
                    return "OrderBy";
            }
        }
    }
}