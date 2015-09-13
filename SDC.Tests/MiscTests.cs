using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDC.data;
using System.Linq;

namespace SDC.Tests
{
    [TestClass]
    public class MiscTests
    {
        public class BookProjection
        {
            public string Title { get; set; }
        }

        [TestMethod]
        public void EFProjections_Test()
        {
            //I want to see if querying the db with projections 
            //will load and keep the objects in the db context.
            //I assume not.
            using (var db = new SDCContext())
            {
                Assert.IsTrue(db.Books.Local.Count == 0);
                var bookProjections = db.Books.Select(b => new BookProjection() { Title = b.Title }).ToList();
                Assert.IsTrue(db.Books.Local.Count == 0);
            }
        }
    }
}
