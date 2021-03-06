﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDC.data;
using System.Linq;
using System.Security.Cryptography;

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

        [TestMethod]
        public void MD5_Test()
        {
            var md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes("a"));
            var s = BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();
        }
    }
}
