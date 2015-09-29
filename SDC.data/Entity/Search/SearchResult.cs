using SDC.data.Entity.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.data.Entity.Search
{
    public enum RelevanceEnum
    {
        FoundInTitle = 1,
        FoundInDescription=2
    }

    /// <summary>
    /// Search results will be stored in the db.
    /// a search result has N entries, both books and authors. (and what may come in the future)
    /// </summary>
    public class BookSearch
    {
        public int Id { get; set; }
        public string Term { get; set; }
        public DateTime Date { get; set; }
        public UserProfile User { get; set; } // can be null
        //public ICollection<SearchResultEntry> Results { get; set; }
  
    }

    public class SearchResultEntry
    {
        public int Id { get; set; }
        public RelevanceEnum Relevance { get; set; }
        public int Rank { get; set; }

        public int ObjectId { get; set; }
    }

    /// <summary>
    /// idea:
    ///     this will not hold references to the object directly. 
    /// reason: 
    ///     if the book is later deleted, search result will hold to it. 
    ///     Should I delete the search result if the item is deleted??!
    /// </summary>
    public class SearchResultBook : SearchResultEntry
    {
        public string Title { get; set; }

        public int OwnerId { get; set; }
        public string OwnerUserName { get; set; }

        public bool IsObsolete { get; set; }
    }

    public class SearchResultAuthor : SearchResultEntry
    {
        public string Name { get; set; }
    }

}
