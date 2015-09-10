using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.data.Entity.Profile
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordRepeat { get; set; }
        public string UserMessage { get; set; }
    }
}