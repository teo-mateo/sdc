using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SDC.data.Entity;
using SDC.Tests.Mocks;
using SDC.web.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SDC.Tests
{
    /// <summary>
    /// base class for all tests that test controllers.
    /// derive the test class from this and call CreateController
    /// </summary>
    public class ControllerTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            SDC.web.AutoMapperConfig.MappingsConfig.RegisterMappings();
        }

        protected ControllerContext GetContext(
            ControllerBase c, 
            NameValueCollection queryString = null)
        {
            //mock the session
            var mockContext = MockRepository.GenerateMock<HttpContextBase>();
            mockContext.Expect(p => p.Session).Return(new MockSession());
            mockContext.Expect(p => p.User).Return(new MockPrincipal());
            mockContext.Expect(p => p.Request).Return(new MockRequest(queryString));
            return new ControllerContext(mockContext,  new RouteData(), c);
        }

        protected T CreateController<T>(NameValueCollection queryString = null) where T : SDCController, new()
        {
            T controller = Activator.CreateInstance<T>();
            controller.ControllerContext = GetContext(controller, queryString);
            return controller;
        }

    }
}
