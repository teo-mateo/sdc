using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.data.Entity.Books
{
    /// <summary>
    /// 
    /// </summary>
    public class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsVerified { get; set; }
        public UserProfile AddedBy { get; set; }
    }
}