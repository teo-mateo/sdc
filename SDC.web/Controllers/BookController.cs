using SDC.data;
using SDC.data.Entity.Books;
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
using System.Net;
using SDC.data.ViewModels;

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
        public ActionResult AddBook(BookViewModel bookViewModel)
        {
            var profile = (UserProfile)Session["UserInfo"];
            if (!User.Identity.IsAuthenticated || profile == null)
                return RedirectToAction("Index", "Home");


            using (var db = new SDCContext())
            using (var t = db.Database.BeginTransaction())
            {
                db.AttachProfile(profile);

                //verify that the shelf exists and it belongs to the logged in user
                var shelf = db.Shelves.Include(o => o.Owner).FirstOrDefault(s => s.Id == bookViewModel.ShelfId);
                if (shelf == null || shelf.Owner.UserId != profile.UserId)
                {
                    //redirect to home?! 
                    //this is not expected to happen, anyway.
                    return RedirectToAction("Index", "Home");
                }

                Book book = AutoMapper.Mapper.Map<Book>(bookViewModel);
                book.Shelf = shelf;
                book.AddedDate = DateTime.Now;



                Book.MapComplexProperties(db, book, bookViewModel, profile);

                db.Books.Add(book);
                db.SaveChanges();
                t.Commit();
            }

            return null;
        }

        [HttpPost]
        public ActionResult UpdateBook(BookViewModel bookViewModel)
        {
            var profile = ((UserProfile)Session["UserInfo"]);
            if (!User.Identity.IsAuthenticated || profile == null)
                return RedirectToAction("Index", "Home");

            try
            {
                using (var db = new SDCContext())
                {
                    db.AttachProfile(profile);

                    var book = db.Books
                        .Include(b=>b.Authors)
                        .Include(b=>b.Genres)
                        .Include(b=>b.Publisher)
                        .Include(b=>b.Language)
                        .First(b => b.Id == bookViewModel.Id);

                    AutoMapper.Mapper.Map<BookViewModel, Book>(bookViewModel, book);

                    Book.MapComplexProperties(db, book, bookViewModel, profile);

                    db.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }
            catch(Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
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