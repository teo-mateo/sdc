using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDC.data.Entity.Profile
{
    public class ChangeEmailModel
    {
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string UserMessage { get; set; }
    }
}