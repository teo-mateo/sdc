using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDC.web.Models;
using SDC.web.ViewModels;

namespace SDC.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CopySimpleProperties_Test()
        {
            var source = new UserProfile();
            var dest = new UserProfileViewModel();

            source.UserId = 11;

            SDC.Library.Tools.CopySimpleProperties.Copy(source, dest);

            Assert.IsTrue(dest.UserId == 11);

        }
    }
}
