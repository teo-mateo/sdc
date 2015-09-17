using SDC.data.Entity.Location;
using SDC.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SDC.data.Entity.Books
{
    public class Book : IEntity
    {
        public Book()
        {
            Authors = new List<Author>();
            Genres = new List<Genre>();
            Pictures = new List<BookPicture>();
        }

        public int Id { get; set; }
        public ICollection<Author> Authors { get; set; }
        public string Title { get; set; }
        public ICollection<Genre> Genres { get; set; }

        public int Year { get; set; }
        public Language Language { get; set; }
        public int? Pages { get; set; }
        public string ISBN { get; set; }
        public Publisher Publisher { get; set; }
        public string Description { get; set; }
        public virtual ICollection<BookPicture> Pictures { get; set; }

        //todo: currencies; link to user profile?
        public decimal? Price { get; set; }
        public DateTime AddedDate { get; set; }
        public Shelf Shelf { get; set; }

        public static void MapComplexProperties(SDCContext db, Book book, BookViewModel bookViewModel, UserProfile profile)
        {
            #region Authors entities
            var auth_to_remove = book.Authors.Where(a => !bookViewModel.Authors.Any(a2 => a2.Id == a.Id)).ToList();
            var auth_to_add = bookViewModel.Authors.Where(a => !book.Authors.Any(a2 => a2.Id == a.Id)).ToList();
            auth_to_remove.ForEach(a => book.Authors.Remove(a));
            auth_to_add.ForEach(a => book.Authors.Add(a));

            foreach (Author a in book.Authors)
            {
                if (a.Id == 0)
                {
                    a.AddedDate = DateTime.Now;
                    a.AddedBy = profile;
                    db.Entry<Author>(a).State = EntityState.Added;
                }
                else
                {
                    db.Attach(a);
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
                db.Attach(g);
            }
            #endregion

            #region Publisher entity

            if (bookViewModel.Publisher != null)
            {
                db.Attach(bookViewModel.Publisher);
                book.Publisher = bookViewModel.Publisher;
            }
            else
                book.Publisher = null;



            #endregion

            #region Language
            var lang = bookViewModel.Language;
            db.AttachCodeEntity(ref lang);
            book.Language = lang;

            #endregion
        }
    }
}