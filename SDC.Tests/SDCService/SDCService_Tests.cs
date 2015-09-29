using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDC.Library.ServiceLayer;
using Ninject;
using SDC.Library;
using SDC.Library.NinjectModules;

namespace SDC.Tests.SDCService
{
    [TestClass]
    public class SDCService_Tests
    {
        [TestInitialize]
        public void Init()
        {
            if (SDCApp.Kernel == null)
            {
                SDCApp.Kernel = new Ninject.StandardKernel(new SDCModule());
            }
        }

        [TestMethod]
        public void SDCService_SimpleBookSearch_Test()
        {
            ISDCService service = SDCApp.Kernel.Get<ISDCService>();
            var result = service.Search("blackberry", null);

            Assert.IsTrue(result.Id > 0);
        }
    }
}
