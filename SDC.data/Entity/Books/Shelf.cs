using SDC.data.Entity.Profile;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDC.data.Entity.Books
{
    public class Shelf
    {
        public int Id { get; set; }
        public virtual UserProfile Owner { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        public bool IsVisible { get; set; }

        public virtual ICollection<Book> Books { get; set; }

        public bool CanBeEdited(UserProfile profile)
        {
            //check that the user owns the shelf or is ADMIN/CURATOR.
            bool userIsOwner = this.Owner.UserId == profile.UserId;
            bool userIsCurator = (profile.Role == RolesCustom.CURATOR || profile.Role == RolesCustom.ADMIN);

            return userIsOwner || userIsCurator;
        }
    }
}