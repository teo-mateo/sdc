using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Tests.Mocks
{
    public class MockPrincipal : IPrincipal
    {
        public IIdentity Identity
        {
            get
            {
                return new MockIdentity();
            }
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}
