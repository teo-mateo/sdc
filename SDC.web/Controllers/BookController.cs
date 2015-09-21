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
using SDC.Library.S3;

namespace SDC.web.Controllers
{
    public class BookController : SDCController
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

            var profile = (UserProfile)Session["UserInfo"];

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

                bool showEditor = false;
                Boolean.TryParse(Request.QueryString["showEditor"], out showEditor);
                ViewBag.ShowEditor = showEditor;

                if(profile != null)
                {
                    ViewBag.Breadcrumbs = Breadcrumb.Generate(
                        "My shelves", Url.Action("Index", "Shelves"),
                        book.Shelf.Name, Url.Action("Details", "Shelves", new { id = book.Shelf.Id }),
                        book.Title, "");
                }
                else
                {
                    ViewBag.Breadcrumbs = Breadcrumb.Generate(
                        book.Shelf.Name, Url.Action("Details", "Shelves", new { id = book.Shelf.Id }),
                        book.Title, "");
                }

                return View(AutoMapper.Mapper.Map<BookViewModel>(book));
            }
        }

        [HttpPost]
        public JsonResult AddBook(BookViewModel bookViewModel)
        {
            var profile = (UserProfile)Session["UserInfo"];
            if (!User.Identity.IsAuthenticated || profile == null)
            {
                //STUPID
                return Json(new { id = -1 });
            }

            int id = 0;

            using (var db = new SDCContext())
            {
                db.AttachProfile(profile);

                //verify that the shelf exists and it belongs to the logged in user
                var shelf = db.Shelves.Include(o => o.Owner).FirstOrDefault(s => s.Id == bookViewModel.ShelfId);
                if (shelf == null || shelf.Owner.UserId != profile.UserId)
                {
                    //STUPID
                    return Json(new { id = -1 });
                }

                Book book = AutoMapper.Mapper.Map<Book>(bookViewModel);
                book.Shelf = shelf;
                book.AddedDate = DateTime.Now;
                Book.MapComplexProperties(db, book, bookViewModel, profile);

                db.Books.Add(book);
                db.SaveChanges();
                id = book.Id;

                //activity
                SDC.Library.Helpers.ActivityHelper.Activity_BookAdded(
                    db, profile, book,
                    Url.Action("ViewBook", "Book", new { id = book.Id }),
                    Url.Action("Details", "Shelves", new { id = book.Shelf.Id }));
            }

            return Json(new { id = id });
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
                        .Include(b=>b.Shelf)
                        .First(b => b.Id == bookViewModel.Id);

                    AutoMapper.Mapper.Map<BookViewModel, Book>(bookViewModel, book);

                    Book.MapComplexProperties(db, book, bookViewModel, profile);

                    db.SaveChanges();

                    //activity
                    SDC.Library.Helpers.ActivityHelper.Activity_BookUpdated(
                        db, profile, book,
                        Url.Action("ViewBook", "Book", new { id = book.Id }),
                        Url.Action("Details", "Shelves", new { id = book.Shelf.Id }));

                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }
            catch(Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult UploadBookPicture(BookImageUploadViewModel model)
        {
            try
            {
                if (model.ImageUpload != null && 
                    model.ImageUpload.ContentLength > 0 && 
                    model.ImageUpload.ContentLength < 1024 * 1024 &&
                    model.UploadForBookId != 0)
                {
                    S3File f = S3.UploadBookImage(
                        model.UploadForBookId.ToString(), 
                        model.ImageUpload.FileName, 
                        model.ImageUpload.InputStream);

                    using(var db = new SDCContext())
                    {
                        var book = db.Books.Include(b => b.Pictures).First(b => b.Id == model.UploadForBookId);
                        book.Pictures.Add(new BookPicture()
                        {
                            Url = f.Url,
                            Key = f.Key,
                            Title = "",
                            IsMain = false
                        });

                        db.SaveChanges();
                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult DeleteBook(int deleteBookId)
        {
            using (var db = new SDCContext())
            {
                var book = db.Books
                    .Include(b=>b.Pictures)
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
                        //fuck this.
                        profile = db.UserProfiles.Find(profile.UserId);
                        
                        //remove book images
                        foreach(var pic in book.Pictures)
                        {
                            db.BookPictures.Remove(pic);
                            S3.DeleteFile(pic.Key);
                        }

                        string shelfName = book.Shelf.Name;

                        db.Books.Remove(book);
                        db.SaveChanges();

                        //activity
                        SDC.Library.Helpers.ActivityHelper.Activity_BookRemoved(db, profile, book, shelfName);

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
                //todo: log
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult DeleteBookPicture (int id)
        {
            try
            {
                var profile = (UserProfile)this.Session["UserInfo"];
                using (var db = new SDCContext())
                {
                    var picture = db.BookPictures
                        .Include(p => p.Book)
                        .Include(p => p.Book.Shelf)
                        .Include(p=>p.Book.Shelf.Owner)
                        .FirstOrDefault(p => p.Id == id);

                    if(picture != null)
                    {
                        if (picture.Book.Shelf.Owner.UserId == profile.UserId ||
                            profile.IsAdmin || profile.IsCurator)
                        {
                            db.BookPictures.Remove(picture);
                            db.SaveChanges();
                            S3.DeleteFile(picture.Key);
                        }
                        else
                        {
                            throw new Exception("Unauthorized");
                        }
                    }
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                //todo: log.
                throw ex;
            }
        }



    }
}