using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDC.web.Controllers;
using SDC.web.Models.Books;

namespace SDC.Tests
{
    [TestClass]
    public class HelperControllerTests
    {
        [TestMethod]
        public void GetPublishersJson_no_searchString_Test()
        {
            HelperController c = new HelperController();
            var res = (Publisher[])c.GetPublishersJson().Data;
            Assert.IsTrue(res.Length > 0);
        }

        [TestMethod]
        public void GetPublishersJson_search_Test()
        {
            HelperController c = new HelperController();
            var res = (Publisher[])c.GetPublishersJson("Nemi").Data;
            Assert.IsTrue(res.Length == 1);
        }

    }
}
