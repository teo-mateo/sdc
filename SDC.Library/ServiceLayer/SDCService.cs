using SDC.data;
using SDC.data.Entity;
using SDC.data.Entity.Search;
using SDC.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Library.ServiceLayer
{
    public class SDCService : ISDCService
    {
        private SearchCache _cache;
        public SDCService()
        {
            _cache = new SearchCache();
        }

        public SearchResultDTO Search(string term, int? userId = null)
        {
            try
            {
                term = term.Trim();
                if (String.IsNullOrWhiteSpace(term) || term.Length < 3)
                {
                    return SearchResultDTO.Empty();
                }

                using (var db = new SDCContext())
                {

                    UserProfile profile = null;
                    if (userId != null)
                        profile = db.UserProfiles.FirstOrDefault(p => p.UserId == (int)userId);

                    var booksResult = db.Books
                        .Where(b => b.Shelf.IsVisible && b.Title.Contains(term))
                        .Select(b => new SearchResultBookDTO()
                         {
                             Id = b.Id,
                             OwnerId = b.Shelf.Owner.UserId,
                             OwnerUserName = b.Shelf.Owner.UserName,
                             OwnerRating = 3.5f,
                             OwnerAvatarUrl = b.Shelf.Owner.Avatar.Url,
                             Title = b.Title,
                             Authors = b.Authors.Select(a => new AuthorDTO()
                             {
                                 Id = a.Id,
                                 Name = a.Name
                             }).ToList(),
                             BookPictures = b.Pictures.Select(p => new BookPictureDTO()
                             {
                                 Url = p.Url
                             }).ToList()
                         }).ToArray();

                    for(int i = 0; i < booksResult.Length; i++)
                    {
                        booksResult[i].Rank = i + 1;
                    }

                    BookSearch search = new BookSearch()
                    {
                        Date = DateTime.Now,
                        Term = term,
                        User = profile
                    };

                    db.BookSearches.Add(search);
                    db.SaveChanges();

                    var result = new SearchResultDTO(search.Id, booksResult, search.Term);

                    _cache.Add(result.Id, result);
                    return result.Subset(0, 10);
                }
            }
            catch (Exception ex)
            {
                //return empty result
                return SearchResultDTO.Empty();
            }
        }

        public SearchResultDTO SearchSubset(int id, int page, int pageSize)
        {
            var result = _cache.Get(id);
            if(result != null)
            {
                return result.Subset(page, pageSize);
            }
            else
            {
                return SearchResultDTO.Empty();
            }
        }


    }


}
