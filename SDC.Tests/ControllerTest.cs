using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SDC.data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SDC.Tests
{
    public class ControllerTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            SDC.web.AutoMapperConfig.MappingsConfig.RegisterMappings();
        }

        protected ControllerContext GetContext(ControllerBase c)
        {
            //mock the session
            var mockContext = MockRepository.GenerateMock<HttpContextBase>();
            mockContext.Expect(p => p.Session).Return(new TestSession());
            mockContext.Expect(p => p.User).Return(new TestPrincipal());
            return new ControllerContext(mockContext, new RouteData(), c);
        }

        protected T CreateController<T>() where T : Controller, new()
        {
            T controller = Activator.CreateInstance<T>();
            controller.ControllerContext = GetContext(controller);
            return controller;
        }


    }


    /// <summary>
    /// I think I should be able to use mock         ... instead of these
    /// 
    /// </summary>
    public class TestPrincipal : IPrincipal
    {
        public IIdentity Identity
        {
            get
            {
                return new TestIdentity();
            }
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }

    public class TestIdentity : IIdentity
    {
        public string AuthenticationType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }



}
