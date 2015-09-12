using SDC.data.Entity.Books;
using SDC.web.ViewModels;
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

namespace SDC.web.Controllers
{
    public class ShelvesController : Controller
    {
        // GET: Shelves
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Index()
        {

            var q = "faccsdfaasf;";
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

        [HttpPost]
        public ActionResult NewShelf(ShelvesViewModel model)
        {
            if (String.IsNullOrEmpty(model.Name))
            {
                return RedirectToAction("Index");
            }

            //save
            using(var db = new SDCContext())
            {
                Shelf newShelf = new Shelf()
                {
                    CreationDate = DateTime.Now,
                    Name = model.Name,
                    IsVisible = model.IsVisible,
                    Owner = db.UserProfiles.Find(((UserProfile)Session["UserInfo"]).UserId)
                };

                db.Shelves.Add(newShelf);
                db.SaveChanges();
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
        public ActionResult Details(int id, int page=0, int pagesize=20)
        {
            var profile = (UserProfile)this.Session["UserInfo"];

            using(var db= new SDCContext())
            {
                var shelf = db.Shelves
                    .Include("Books")
                    .Include(a => a.Books.Select(b=>b.Authors))
                    .Include(a => a.Books.Select(b => b.Genres))
                    .Include(a => a.Books.Select(b=>b.Publisher))
                    .FirstOrDefault(p => p.Id == id);
                if (shelf == null)
                    return RedirectToAction("Index");

                if (!shelf.CanBeEdited(profile))
                {
                    return RedirectToAction("Index", "Home");
                }

                var vm = new ShelfViewModel()
                {
                    Id = shelf.Id,
                    Name = shelf.Name,
                    IsVisible = shelf.IsVisible,
                    BookCount = shelf.Books.Count(),
                    Books = shelf.Books.ToList(),
                    Languages = Language.GetAll(db), 
                    Genres = Genre.GetAll(db)
                };
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ViewBook(int id = 0)
        {
            if (id == 0) //this should not happen
                return RedirectToAction("Index", "Home");

            using(var db = new SDCContext())
            {
                var book = db.Books
                    .Include(b=>b.Shelf)
                    .Include(b => b.Authors)
                    .Include(b => b.Genres)
                    .Include(b => b.Publisher)
                    .Include(b=> b.Pictures)
                    .First(b => b.Id == id);
                return View(book);
            }
            
        }

        [HttpPost]
        public ActionResult AddBook(BookViewModel book)
        {
            var profile = (UserProfile)Session["UserInfo"];
            if (profile == null)
                return RedirectToAction("Index", "Home");

            using (var db = new SDCContext())
            using(var t = db.Database.BeginTransaction())
            {
                //verify that the shelf exists and it belongs to the logged in user
                var shelf = db.Shelves.Include(o=>o.Owner).FirstOrDefault(s=>s.Id == book.ShelfId);
                if (shelf == null || shelf.Owner.UserId != profile.UserId)
                {
                    //redirect to home?! 
                    //this is not expected to happen, anyway.
                    return RedirectToAction("Index", "Home");
                }

                profile = db.UserProfiles.Find(profile.UserId);
                Book newBook = new Book();
                newBook.Shelf = shelf;


                //add existing authors
                var authors_ids = book.Authors.Where(a=>a.Id != 0)
                    .Select(a => a.Id);
                var existing_authors = (from a in db.Authors
                                         where authors_ids.Contains(a.Id)
                                         select a);

                foreach (var a in existing_authors)
                {
                    if(a != null)
                        newBook.Authors.Add(a);
                }

                //add new authors
                var new_authors = (from a in book.Authors
                                   where a.Id == 0
                                   select a);
                foreach (var a in new_authors)
                {
                    a.AddedBy = profile;
                    newBook.Authors.Add(a);
                }


                //genre
                if (book.Genres != null)
                {
                    foreach(var g in book.Genres)
                    {
                        newBook.Genres.Add(db.Genres.Find(g.Id));
                    }
                }

                //publisher
                if(book.Publisher != null)
                {
                    if(book.Publisher.Id == 0)
                    {
                        book.Publisher.AddedBy = profile;
                        db.Publishers.Add(book.Publisher);
                        db.SaveChanges();
                        newBook.Publisher = book.Publisher;
                    }
                    else
                    {
                        newBook.Publisher = db.Publishers.Find(book.Publisher.Id);
                    }

                    
                }

                //simple properties
                newBook.Title = book.Title;
                newBook.Year = book.Year;
                newBook.ISBN = book.ISBN;
                newBook.Language = book.Language.Code;
                newBook.AddedDate = DateTime.Now;

                db.Books.Add(newBook);
                db.SaveChanges();
                t.Commit();
            }

            return null;
        }

        [HttpPost]
        public ActionResult ChangeBook(BookViewModel book)
        {
            throw new NotImplementedException();
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

        [HttpPost]
        public ActionResult DeleteBook(int deleteBookId)
        {
            using (var db = new SDCContext())
            {
                var book = db.Books
                    .Include(b=>b.Shelf)
                    .Include(b=>b.Shelf.Owner)
                    .FirstOrDefault(b=>b.Id == deleteBookId);
                if(book != null)
                {
                    var shelfId = book.Shelf.Id;

                    // only admin, curator or shelf owner can delete it.
                    var profile = (UserProfile)Session["UserInfo"];
                    if( profile.Role == RolesCustom.ADMIN || 
                        profile.Role == RolesCustom.CURATOR ||
                        book.Shelf.Owner.UserId == profile.UserId)
                    {
                        db.Books.Remove(book);
                        db.SaveChanges();
                        return RedirectToAction("Details", "Shelves", new { id = shelfId });
                    }
                }
            }
            //any other case
            return RedirectToAction("Index", "Home");
        }

    }
}