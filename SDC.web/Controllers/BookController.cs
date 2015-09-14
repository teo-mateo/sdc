using SDC.data;
using SDC.data.Entity.Books;
using SDC.web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SDC.data.Entity;
using SDC.data.Entity.Profile;
using AutoMapper.QueryableExtensions;
using SDC.data.Entity.Location;

namespace SDC.web.Controllers
{
    public class BookController : Controller
    {
        /// <summary>
        /// not to be used. 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ViewBook(int id = 0)
        {
            if (id == 0) //this should not happen
                return RedirectToAction("Index", "Home");

            

            using (var db = new SDCContext())
            {
                ViewBag.Languages = db.Languages.Where(p=>p.IsVisible).OrderBy(p=>p.Code).ToList();
                ViewBag.Genres = db.Genres.OrderBy(p=>p.Name).ToList();

                var book = db.Books
                    .Include(b => b.Shelf)
                    .Include(b => b.Shelf.Owner)
                    .Include(b => b.Authors)
                    .Include(b => b.Genres)
                    .Include(b => b.Publisher)
                    .Include(b => b.Pictures)
                    .First(b => b.Id == id);
                return View(AutoMapper.Mapper.Map<BookViewModel>(book));
            }
        }

        [HttpPost]
        public ActionResult AddBook(BookViewModel book)
        {
            var profile = (UserProfile)Session["UserInfo"];
            if (profile == null)
                return RedirectToAction("Index", "Home");

            using (var db = new SDCContext())
            using (var t = db.Database.BeginTransaction())
            {
                //verify that the shelf exists and it belongs to the logged in user
                var shelf = db.Shelves.Include(o => o.Owner).FirstOrDefault(s => s.Id == book.ShelfId);
                if (shelf == null || shelf.Owner.UserId != profile.UserId)
                {
                    //redirect to home?! 
                    //this is not expected to happen, anyway.
                    return RedirectToAction("Index", "Home");
                }

                profile = db.UserProfiles.Find(profile.UserId);
                Book newBook = AutoMapper.Mapper.Map<Book>(book);
                newBook.Shelf = shelf;
                newBook.AddedDate = DateTime.Now;

                //add genres to the db context
                if (newBook.Genres != null)
                    foreach (var g in newBook.Genres)
                    {
                        //right way
                        db.Genres.Attach(g);
                        db.Entry<Genre>(g).State = EntityState.Unchanged;
                    }

                //add authors to the db context
                if (newBook.Authors != null)
                {
                    foreach (var a in newBook.Authors)
                    {
                        //I am only attaching the existing authors.
                        //for the new ones, they should be added.
                        if (a.Id != 0)
                        {
                            db.Authors.Attach(a);
                            db.Entry<Author>(a).State = EntityState.Unchanged;
                        }
                        else
                        {
                            a.IsVerified = false;
                            a.AddedBy = profile;
                        }
                    }
                }

                if(newBook.Language != null)
                {
                    db.Languages.Attach(newBook.Language);
                    db.Entry<Language>(newBook.Language).State = EntityState.Unchanged;
                }

                //add publisher to the db context
                if (newBook.Publisher != null)
                {
                    db.Publishers.Attach(newBook.Publisher);
                    db.Entry<Publisher>(newBook.Publisher).State = EntityState.Unchanged;
                }

                db.Books.Add(newBook);
                db.SaveChanges();
                t.Commit();
            }

            return null;
        }

        [HttpPost]
        public ActionResult UpdateBook(BookViewModel book)
        {
            throw new NotImplementedException();
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

        [HttpGet]
        public JsonResult GetBookJson(int id)
        {
            try {
                using (var db = new SDCContext())
                {
                    //feel like doing some projections? :D
                    //this will be more useful in the future
                    //when a book will have other entities attached to it
                    // such as transactions.
                    BookViewModel bookViewModel = db.Books.AsQueryable().Project()
                        .To<BookViewModel>()
                        .First(b => b.Id == id);

                    return Json(bookViewModel, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }



    }
}