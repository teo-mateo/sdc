using SDC.data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SDC.Tests
{
    public class TestSession : HttpSessionStateBase
    {
        private Dictionary<string, object> _dict = new Dictionary<string, object>();

        public TestSession()
        {
            _dict.Add("UserInfo", new UserProfile() { UserId = 1, UserName = "admin" });
        }

        public override object this[string name]
        {
            get
            {
                return _dict[name];
            }

            set
            {
                _dict[name] = value;
            }
        }
    }
}
