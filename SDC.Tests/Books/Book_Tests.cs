using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDC.data;
using System.Linq;
using SDC.data.ViewModels;
using SDC.data.Entity.Books;
using SDC.data.Entity.Location;
using SDC.web.Controllers;
using Rhino.Mocks;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Collections.Generic;
using SDC.data.Entity;
using System.Data.Entity;

namespace SDC.Tests.Books
{
    [TestClass]
    public class Book_Tests : ControllerTest
    {

        [TestMethod]
        public void AddBook_Test()
        {
            BookController c = CreateController<BookController>();
            BookViewModel vm; int bookCount;

            using (var db = new SDCContext())
            {
                bookCount = db.Books.Count();

                var profile = db.UserProfiles.Find(1);
                var shelf = db.Shelves.FirstOrDefault(p => p.Owner.UserId == profile.UserId);
                var twoGenres = db.Genres.Take(2)
                    .ToList();

                var twoAuthors = db.Authors.OrderBy(a => Guid.NewGuid().ToString())
                    .Take(2)
                    .ToList();

                var publisher = db.Publishers.Take(1)
                    .First();

                var language = db.Languages
                    .Where(l=>l.IsVisible)
                    .OrderBy(l => Guid.NewGuid().ToString())
                    .First();

                vm = new BookViewModel()
                {
                    Title = Guid.NewGuid().ToString(),
                    Year = 2015,
                    Genres = twoGenres,
                    Authors = twoAuthors,
                    Description = "Lorem ipsum",
                    Publisher = publisher,
                    Language = language,
                    ShelfId = shelf.Id,
                    ShelfName = shelf.Name,
                    ISBN = Guid.NewGuid().ToString(),
                    AddedDate = DateTime.Now
                };  
            }

            
            c.AddBook(vm);
            Assert.AreEqual(bookCount + 1, new SDCContext().Books.Count());
            
        }

        [TestMethod]
        public void UpdateBook_Test()
        {
            BookController c = base.CreateController<BookController>();
            BookViewModel vm;
            int id;
            using (var db = new SDCContext())
            {
                //will get last book of admin's first shelf

                var profile = db.UserProfiles.Find(1);
                var shelf = db.Shelves.FirstOrDefault(p => p.Owner.UserId == profile.UserId);
                var book_original = db.Books
                    .Include(b=>b.Authors)
                    .Include(b=>b.Genres)
                    .Include(b=>b.Publisher)
                    .Include(b=>b.Language)
                    .OrderByDescending(b => b.AddedDate)
                    .Where(p => p.Shelf.Id == shelf.Id && p.Authors.Count > 0 && p.Genres.Count > 0)
                    .First();
                id = book_original.Id;

                vm = AutoMapper.Mapper.Map<BookViewModel>(book_original);

                //changes: title, year, remove author, add an author, remove a genre, add a genre,
                //description

                string title = Guid.NewGuid().ToString();
                string desc = Guid.NewGuid().ToString();
                int year = 2000;

                var auth_add = db.Authors.OrderBy(a => Guid.NewGuid()).First();
                var genre_add = db.Genres.OrderBy(a => Guid.NewGuid()).First();

                //remove an author, add the other one.
                var removed_author = vm.Authors.Last();
                vm.Authors.Remove(removed_author);
                vm.Authors.Add(auth_add);

                //remove a genre, add the other one.
                var removed_genre = vm.Genres.Last();
                vm.Genres.Remove(removed_genre);
                vm.Genres.Add(genre_add);

                //change language
                var newLanguage = db.Languages.First(l => l.IsVisible && l.Code != book_original.Language.Code);
                vm.Language = newLanguage;

                vm.Title = title;
                vm.Description = desc;
                vm.Year = year;

                bool hasPublisher = vm.Publisher != null;
                if (hasPublisher)
                    vm.Publisher = null;
                else
                    vm.Publisher = db.Publishers.First();

                //act
                c.UpdateBook(vm);

                var updated_book = db.Books.AsNoTracking()
                    .Include(b => b.Authors)
                    .Include(b => b.Genres)
                    .Include(b=>b.Publisher)
                    .Include(b=>b.Language)
                    .First(b => b.Id == id);

                //asserts
                Assert.IsFalse(updated_book.Authors.Any(p => p.Id == removed_author.Id));
                Assert.IsTrue(updated_book.Authors.Any(p => p.Id == auth_add.Id));

                Assert.IsFalse(updated_book.Genres.Any(p => p.Id == removed_genre.Id));
                Assert.IsTrue(updated_book.Genres.Any(p => p.Id == genre_add.Id));


                Assert.IsTrue(updated_book.Title.Equals(title));
                Assert.IsTrue(updated_book.Year.Equals(year));
                Assert.IsTrue(updated_book.Description.Equals(desc));

                if (hasPublisher)
                    Assert.IsTrue(updated_book.Publisher == null);
                else
                    Assert.IsTrue(updated_book.Publisher != null);

                Assert.AreEqual(updated_book.Language.Code, newLanguage.Code);
            }


        }
    }
}
