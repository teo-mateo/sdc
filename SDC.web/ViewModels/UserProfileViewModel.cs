using SDC.data.Entity.Location;
using SDC.data.Entity.Profile;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public Country Country { get; set; }
        public City City { get; set; }
        public Avatar CustomAvatar { get; set; }

        public string Password { get; set; }
        public string Message { get; set; }

        public string Role { get; set; }


        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageUpload { get; set; }

        public bool ShowEmail { get; set; }

        public UserProfileViewModel()
        {
            this.Avatar = new Avatar();
            this.Country = new Country();
            this.City = new City();

        }

        public Avatar[] DefaultAvatars { get; set; }
        public City[] AllCities { get; set; }
        public Country[] AllCountries { get; set; }
    }
}