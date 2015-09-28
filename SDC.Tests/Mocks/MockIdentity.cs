using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Tests.Mocks
{
    public class MockIdentity : IIdentity
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
