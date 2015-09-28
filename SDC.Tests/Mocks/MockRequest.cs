using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SDC.Tests.Mocks
{
    public class MockRequest : HttpRequestBase
    {
        private NameValueCollection _queryString;

        public MockRequest(NameValueCollection queryString) : base()
        {
            _queryString = queryString;   
        }

        public MockRequest() : base()
        {

        }

        public override NameValueCollection QueryString
        {
            get
            {
                return _queryString;
            }
        }
    }
}
