using SDC.web.Models;
using SDC.web.Models.Books;
using SDC.web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDC.web.Controllers
{
    public class ShelvesController : Controller
    {
        // GET: Shelves
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Index()
        {
            var userProfile = (UserProfile)this.Session["UserInfo"];

            using (var db = new SDCContext())
            {
                var shelves = (from s in db.Shelves
                               orderby s.Name
                               where s.Owner.UserId == userProfile.UserId
                               select new ShelfViewModel()
                               {
                                   Id = s.Id,
                                   Name = s.Name,
                                   CreationDate = s.CreationDate,
                                   IsVisible = s.IsVisible,
                                   BookCount = 0
                               }).ToList();

                return View(new ShelvesViewModel()
                {
                    Shelves = shelves
                });
            }

                
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            return View("Details");
        }

        [HttpPost]
        public ActionResult NewShelf(ShelvesViewModel model)
        {
            //save
            using(var db = new SDCContext())
            {
                Shelf newShelf = new Shelf()
                {
                    CreationDate = DateTime.Now,
                    Name = model.NewShelfName,
                    IsVisible = true,
                    Owner = db.UserProfiles.Find(((UserProfile)Session["UserInfo"]).UserId)
                };

                db.Shelves.Add(newShelf);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteShelf(ShelvesViewModel model)
        {
            int id = model.DeleteShelfId;

            //first, check that the user owns the shelf.
            using (var db = new SDCContext())
            {
                var shelf = db.Shelves.Find(id);
                if (shelf == null)
                    return RedirectToAction("Index");

                var userProfile = (UserProfile)this.Session["UserInfo"];

                bool userIsOwner = shelf.Owner.UserId == userProfile.UserId;
                bool userIsCurator = (userProfile.Role == RolesCustom.CURATOR ||
                    userProfile.Role == RolesCustom.ADMIN);

                if (userIsOwner || userIsCurator)
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
    }
}