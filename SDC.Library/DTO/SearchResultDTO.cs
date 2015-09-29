using SDC.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Library.DTO
{
    public class SearchResultViewModel
    {
        public PaginationViewModel Pagination { get; set; }
        public SearchResultDTO Result { get; set; }

        public SearchResultViewModel(SearchResultDTO dto)
        {
            this.Result = dto;
            this.Pagination = new PaginationViewModel()
            {
                Controller = "Search",
                Action = "BookSearch",
                EntityCount = dto.Results.Count,
                EntityName = "Results",
                Page = dto.Page,
                PageSize = 10,
                TotalPages = (int)(dto.Total / 10)
            };
        }
    }

    public class SearchResultDTO
    {
        public int Id { get; set; }
        public string SearchTerm { get; set; }
        public ICollection<SearchResultEntryDTO> Results { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }

        public SearchResultDTO(int id, ICollection<SearchResultEntryDTO> results, string searchTerm)
        {
            this.Id = id;
            this.Results = results;
            this.SearchTerm = searchTerm;
            this.Total = results.Count;
        }

        public SearchResultDTO()
        {

        }

        public static SearchResultDTO Empty()
        {
            //return empty result
            return new SearchResultDTO()
            {
                Id = -1,
                Results = new SearchResultEntryDTO[0],
                SearchTerm = "", 
                Total = 0
            };
        }

        internal SearchResultDTO Subset(int page, int pageSize)
        {
            if (this.Results.Count < (page * pageSize))
                return SearchResultDTO.Empty();

            return new SearchResultDTO()
            {
                Id = this.Id,
                SearchTerm = this.SearchTerm,
                Results = this.Results.Skip(page * pageSize).Take(pageSize).ToArray(), 
                Page = page+1,
                Total = this.Total
            };
        }
    }

    public class SearchResultEntryDTO 
    {
        public int Rank { get; set; }
    }

    public class SearchResultBookDTO : SearchResultEntryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IList<AuthorDTO> Authors { get; set; }
        public int OwnerId { get; set; }
        public string OwnerUserName { get; set; }
        public string OwnerAvatarUrl { get; set; }
        public float OwnerRating { get; set; }
        public IList<BookPictureDTO> BookPictures { get; set; }
    }

    public class SearchResultAuthorDTO : SearchResultEntryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AuthorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BookPictureDTO
    {
        public string Url { get; set; }
    }
}
