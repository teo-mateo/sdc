using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDC.web.Controllers;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;

namespace SDC.Tests.Authors
{
    [TestClass]
    public class AuthorsContorller_Tests : ControllerTest
    {
        [TestMethod]
        public void AuthorsController_GetAllAuthorsJson_GetAll()
        {
            NameValueCollection queryString = new NameValueCollection();
            queryString.Add("onlyWithBooks", "false");
            queryString.Add("start", "0");
            queryString.Add("length", "10");
            queryString.Add("draw", "1");
            queryString.Add("search[value]", "");
            queryString.Add("order[0][column]", "0");
            queryString.Add("order[0][dir]", "asc");

            AuthorsController c = CreateController<AuthorsController>(queryString);
            var result = c.GetAllAuthorsJson();
        }

        [TestMethod]
        public void AuthorsController_GetAllAuthorsJson_OrderByBooksCount()
        {
            NameValueCollection queryString = new NameValueCollection();
            queryString.Add("onlyWithBooks", "false");
            queryString.Add("start", "0");
            queryString.Add("length", "10");
            queryString.Add("draw", "1");
            queryString.Add("search[value]", "");
            queryString.Add("order[0][column]", "3");
            queryString.Add("order[0][dir]", "asc");

            AuthorsController c = CreateController<AuthorsController>(queryString);
            var result = c.GetAllAuthorsJson();
        }
    }
}
