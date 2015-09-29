using SDC.data;
using SDC.data.Entity;
using SDC.data.Entity.Search;
using SDC.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.ServiceLayer
{
    public class SDCService : ISDCService
    {
        public SearchResultDTO Search(string term, int? userId)
        {
            term = term.Trim();
            if(String.IsNullOrWhiteSpace(term) || term.Length < 3)
            {
                //return empty result
                return new SearchResultDTO()
                {
                    Id = -1,
                    Results = new SearchResultEntryDTO[0],
                    SearchTerm = null
                };
            }

            using (var db = new SDCContext())
            {
                //simple stuff: 
                //return books that contain the term in their title.

                UserProfile profile = null;
                if (userId != null)
                    profile = db.UserProfiles.FirstOrDefault(p => p.UserId == (int)userId);



                var booksResult = (from b in db.Books
                                   where b.Shelf.IsVisible && b.Title.Contains(term)
                                   select new SearchResultBookDTO()
                                   {
                                       Id = b.Id,
                                       OwnerId = b.Shelf.Owner.UserId,
                                       OwnerUserName = b.Shelf.Owner.UserName,
                                       Title = b.Title,
                                       Authors = b.Authors.Select(a => new AuthorDTO()
                                       {
                                           Id = a.Id,
                                           Name = a.Name
                                       }).ToList()
                                   }).ToArray();

                BookSearch search = new BookSearch()
                {
                    Date = DateTime.Now,
                    Term = term,
                    User = profile
                };

                db.BookSearches.Add(search);

                return new SearchResultDTO()
                {
                    Id = search.Id,
                    Results = booksResult,
                    SearchTerm = term
                };
            }
        }
    }

    public interface ISDCService
    {
        SearchResultDTO Search(string term, int? userid);
    }
}
