using SDC.web.Models;
using SDC.web.Models.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.web.ViewModels
{
    public class UserProfileViewModel 
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime LastSeen { get; set; }
        public bool IsLocked { get; set; }
        public Avatar Avatar { get; set; }
        public string ComputedProperty { get; set; }
        public City City { get; set; }

        public bool ShowEmail { get; set; }

        public UserProfileViewModel()
        {
            this.Avatar = new Avatar();
            this.City = new City();
            this.DefaultAvatars = new List<Avatar>();
        }

        public List<Avatar> DefaultAvatars { get; set; }
        public List<City> AllCities { get; set; }
    }
}