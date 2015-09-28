using SDC.data.Entity.Books;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SDC.data.Entity;
using SDC.data;
using SDC.data.Entity.Location;
using SDC.data.Entity.Profile;
using SDC.data.ViewModels;

namespace SDC.web.Controllers
{
    public class ShelvesController : SDCController
    {
        // GET: Shelves
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Index()
        {
            ViewBag.Breadcrumbs = Breadcrumb.Generate(
                "My shelves", "");

            var userProfile = (UserProfile)this.Session["UserInfo"];

            if(userProfile == null)
            {
                return RedirectToAction("Index", "Home");
            }

            using (var db = new SDCContext())
            {
                var shelves = (from s in db.Shelves
                               orderby s.Name
                               where s.Owner.UserId == userProfile.UserId
                               select s).ToList();

                var shelvesVMs = shelves.Select(s =>
                {
                    return new ShelfViewModel()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        CreationDate = s.CreationDate,
                        IsVisible = s.IsVisible,
                        BookCount = db.Entry(s).Collection(p=>p.Books).Query().Count()
                    };
                }).ToArray();
                return View(new ShelvesViewModel()
                {
                    Shelves = shelvesVMs
                });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult NewShelf(ShelvesViewModel model)
        {
            if (String.IsNullOrEmpty(model.Name))
            {
                return RedirectToAction("Index");
            }

            UserProfile profile = null;

            //save
            using (var db = new SDCContext())
            {
                profile = db.UserProfiles.Find(((UserProfile)Session["UserInfo"]).UserId);

                Shelf newShelf = new Shelf()
                {
                    CreationDate = DateTime.Now,
                    Name = model.Name,
                    IsVisible = model.IsVisible,
                    Owner = profile
                };

                db.Shelves.Add(newShelf);
                db.SaveChanges();
                Session["UserInfoEx"] = profile.GetExtendedInfo(db);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditShelf(ShelvesViewModel model)
        {
            if (String.IsNullOrEmpty(model.Name))
                return RedirectToAction("Index");

            int id = model.EditShelfId;
            using(var db = new SDCContext())
            {
                var shelf = db.Shelves.Find(id);
                if (shelf == null)
                    return RedirectToAction("Index");

                var userProfile = (UserProfile)this.Session["UserInfo"];
                if (shelf.CanBeEdited(userProfile))
                {
                    shelf.Name = model.Name;
                    shelf.IsVisible = model.IsVisible;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

        }

        [HttpPost]
        public ActionResult DeleteShelf(ShelvesViewModel model)
        {
            int id = model.DeleteShelfId;
            using (var db = new SDCContext())
            {
                //sanity check: this shelf exists.
                var shelf = db.Shelves.Find(id);
                if (shelf == null)
                    return RedirectToAction("Index");

                var userProfile = (UserProfile)this.Session["UserInfo"];

                if (shelf.CanBeEdited(userProfile))
                {
                    //we allow deletion 

                    //delete all books in this shelf.
                    //todo: delete all other entities that are linked
                    var books = (from b in db.Books
                                 where b.Shelf.Id == shelf.Id
                                 select b).ToList();
                    db.Books.RemoveRange(books);
                    db.Shelves.Remove(shelf);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    //bad user, bad!
                    return RedirectToAction("Index");
                }
            }
        }

        /// <summary>
        /// Details of a shelf. books that it contains
        /// </summary>
        /// <param name="id">id of the shelf</param>
        /// <returns>view</returns>
        [HttpGet]
        public ActionResult Details(int id, int page=1, int pagesize = 0)
        {
            if (id == 0)
                return RedirectToAction("Index", "Home");

            var profile = (UserProfile)this.Session["UserInfo"];
            if(profile == null)
                return RedirectToAction("Index", "Home");

            if (pagesize < 1 || pagesize > 100)
                pagesize = profile.PageSize;

            ShelfViewModel vm = null;

            using (var db = new SDCContext())
            {
                profile.UpdatePageSize(db, pagesize);

                ViewBag.Languages = db.Languages.Where(p => p.IsVisible).OrderBy(p => p.Code).ToList();
                ViewBag.Genres = db.Genres.OrderBy(p => p.Name).ToList();


                var shelf = db.Shelves
                    .AsNoTracking()
                    .Include(a => a.Books)
                    .Include(a => a.Books.Select(b => b.Pictures))
                    .Include(a => a.Books.Select(b => b.Authors))
                    .Include(a => a.Books.Select(b => b.Genres))
                    .Include(a => a.Books.Select(b => b.Publisher))
                    .FirstOrDefault(p => p.Id == id);
                if (shelf == null)
                    return RedirectToAction("Index", "Home");

                int totalPages = ((int)Math.Ceiling((double)shelf.Books.Count / pagesize));
                if (page > totalPages)
                    page = totalPages;

                //actual pagination takes place here
                var show_books = shelf.Books
                        .OrderBy(b => b.AddedDate)
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .Select(b => AutoMapper.Mapper.Map<BookViewModel>(b));

                vm = new ShelfViewModel()
                {
                    Id = shelf.Id,
                    Name = shelf.Name,
                    IsVisible = shelf.IsVisible,
                    CanEdit = shelf.CanBeEdited(profile),
                    BookCount = shelf.Books.Count(),
                    Books = show_books,
                    Languages = Language.GetAll(db),
                    Genres = Genre.GetAll(db),
                    DefaultLanguage = profile.Country.Language, 
                    Pagination = new PaginationViewModel()
                    {
                        Id = shelf.Id,
                        Action = "Details", 
                        Controller = "Shelves",
                        Page = page, 
                        PageSize=pagesize,
                        TotalPages = totalPages,
                        EntityCount = show_books.Count(),
                        EntityName = "Books"
                    }
                };

                if (profile.UserId == shelf.Owner.UserId)
                {
                    ViewBag.Breadcrumbs = Breadcrumb.Generate(
                        "My shelves", Url.Action("Index", "Shelves"),
                        vm.Name, "");
                }
                else
                {
                    ViewBag.Breadcrumbs = Breadcrumb.Generate("Directory", "", vm.Name, "");
                }

                db.Dispose();
            }

            return View(vm);
        }


        [HttpGet]
        public JsonResult GetNewBook()
        {
            BookViewModel b = new BookViewModel()
            {
                Title = "new book",
                Authors = new List<Author>()
                {
                    new Author() { Id = 0, Name = "Mihail Sadoveanu" }
                },
                Publisher = new Publisher() { Id = 0, Name = "Pearson, Specter, Litt"},
                Genres = new List<Genre>()
                {
                    new Genre() {Id = 1, Name = "Science fiction" }
                },
                ISBN = "ISBN UNKNOWN",
                Year = 1984
            };

            return Json(b, JsonRequestBehavior.AllowGet);
        }
    }
}