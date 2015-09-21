using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.data.Entity.Profile
{
    public enum ActivityType
    {
        AddBook, 
        UpdateBook, 
        RemoveBook,
        AddShelf,
        RemoveShelf,
        ExchangeInitiated,
        ExchangeCompleted
    }
    public class Activity
    {
        public int Id { get; set; }
        public UserProfile Profile { get; set; }
        public string Content { get; set; }
        public ActivityType Type { get; set; }
    }
}
