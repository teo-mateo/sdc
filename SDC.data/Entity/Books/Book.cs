using SDC.data.Entity.Location;
using SDC.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SDC.data.Entity.Books
{
    public class Book
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

        public static void MapComplexProperties(SDCContext db, Book book, BookViewModel bookViewModel)
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
        }
    }
}