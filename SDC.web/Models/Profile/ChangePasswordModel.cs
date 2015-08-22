using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.Models.Profile
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordRepeat { get; set; }
    }
}