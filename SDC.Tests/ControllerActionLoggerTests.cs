using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDC.web.Controllers;
using System.Threading;
using ServiceStack.Redis;
using System.Linq;
using SDC.Library;
using SDC.Library.NinjectModules;
using SDC.Library.Controllers;

namespace SDC.Tests
{
    [TestClass]
    public class ControllerActionLoggerTests
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
        public void ControllerActionLogger_Test()
        {
            ControllerActionLogger logger = new ControllerActionLogger();
            logger.StartAction("identifier1");
            Thread.Sleep(1234);
            logger.EndAction("identifier1", "theactionname", "thecontrollername", "someinfo");

            RedisClient cli = new RedisClient();
            var set = cli.Sets[logger.SetName];
            var last = set.Pop();
            
        }
    }
}
