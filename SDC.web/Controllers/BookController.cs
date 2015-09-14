﻿using SDC.data;
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
using System.Net;

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
        public ActionResult UpdateBook(BookViewModel bookViewModel)
        {
            var profile = ((UserProfile)Session["UserInfo"]);
            if (!User.Identity.IsAuthenticated || profile == null)
                return RedirectToAction("Index", "Home");

            try
            {
                using (var db = new SDCContext())
                {
                    var book = db.Books.AsNoTracking()
                        .Include(b=>b.Authors)
                        .Include(b=>b.Genres)
                        .Include(b=>b.Publisher)
                        .First(b => b.Id == bookViewModel.Id);

                    AutoMapper.Mapper.Map<BookViewModel, Book>(bookViewModel, book);

                    db.Books.Attach(book);
                    db.Entry<Book>(book).State = EntityState.Modified;

                    #region Authors entities
                    var auth_to_remove = book.Authors.Where(a => !bookViewModel.Authors.Any(a2 => a2.Id == a.Id)).ToList();
                    var auth_to_add = bookViewModel.Authors.Where(a => !book.Authors.Any(a2 => a2.Id == a.Id)).ToList();
                    auth_to_remove.ForEach(a=>book.Authors.Remove(a));
                    auth_to_add.ForEach(a => book.Authors.Add(a));

                    foreach (Author a in book.Authors)
                    {
                        if (a.Id == 0)
                        {
                            db.Entry<Author>(a).State = EntityState.Added;
                        }
                        else
                        {
                            if (db.Set<Author>().Local.Any(local => a == local))
                            {
                                db.Entry<Author>(a).State = EntityState.Unchanged;
                            }
                            else
                            {
                                db.Set<Author>().Attach(a);
                                db.Entry<Author>(a).State = EntityState.Unchanged;
                            }
                        }
                    }
                    #endregion

                    #region Genres entities
                    var genres_to_remove = book.Genres.Where(g => !bookViewModel.Genres.Any(g2 => g2.Id == g.Id)).ToList();
                    var genres_to_add = bookViewModel.Genres.Where(g => !book.Genres.Any(g2 => g2.Id == g.Id)).ToList();
                    genres_to_remove.ForEach(g => book.Genres.Remove(g));
                    genres_to_add.ForEach(g => book.Genres.Add(g));

                    foreach (var g in book.Genres)
                    {
                        if (db.Set<Genre>().Local.Any(local => g == local))
                        {
                            db.Entry<Genre>(g).State = EntityState.Unchanged;
                        }
                        else
                        {
                            db.Set<Genre>().Attach(g);
                            db.Entry<Genre>(g).State = EntityState.Unchanged;
                        }

                    }
                    #endregion

                    #region Publisher entity

                    book.Publisher = bookViewModel.Publisher;
                    
                    if (book.Publisher != null)
                    {
                        if (db.Set<Publisher>().Local.Any(local => book.Publisher == local))
                        {
                            db.Entry<Publisher>(book.Publisher).State = EntityState.Unchanged;
                        }
                        else
                        {
                            db.Set<Publisher>().Attach(book.Publisher);
                            db.Entry<Publisher>(book.Publisher).State = EntityState.Unchanged;
                        }

                        db.Publishers.Attach(book.Publisher);
                        db.Entry<Publisher>(book.Publisher).State = EntityState.Unchanged;
                    } 
                    #endregion

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