using SDC.data.Entity.Location;
using SDC.data.Entity.Profile;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Data;
using System.Linq;
using SDC.data.Entity.Books;
using System.Dynamic;
using System.Web;

namespace SDC.data.Entity
{

    [Table("UserProfile")]
    public class UserProfile 
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public DateTime LastSeen { get; set; }
        public DateTime Created { get; set; }

        public bool IsLocked { get; set; }
        public Avatar Avatar { get; set; }

        public Country Country { get; set; }
        public City City { get; set; }

        public bool ShowEmail { get; set; }

        
        public bool IsAdmin
        {
            get { return Role == RolesCustom.ADMIN; }
        }
        public bool IsCurator
        {
            get { return Role == RolesCustom.CURATOR; }
        }

        public int PageSize { get; set; }

        public static void AttachToContext(UserProfile profile, SDCContext db)
        {
            if (db.Set<UserProfile>().Local.Any(local => profile == local))
            {
                db.Entry<UserProfile>(profile).State = EntityState.Unchanged;
            }
            else
            {
                db.Set<UserProfile>().Attach(profile);
                db.Entry<UserProfile>(profile).State = EntityState.Unchanged;
            }
        }
        [NotMapped]
        public List<Shelf> Shelves { get; set; }
        [NotMapped]
        public string Role { get; set; }

        public dynamic GetExtendedInfo(SDCContext db)
        {
            var shelves = (from s in db.Shelves
                           join b in db.Books on s.Id equals b.Shelf.Id into sb
                           where s.Owner.UserId == UserId
                           select new
                           {
                               Id = s.Id,
                               Name = s.Name,
                               Books = sb.Count()
                           }).ToArray();

            dynamic info = new ExpandoObject();
            info.Shelves = shelves.Select(s =>
            {
                dynamic shelf = new ExpandoObject();
                shelf.Id = s.Id;
                shelf.Name = s.Name;
                shelf.Books = s.Books;
                return shelf;
            });
            return info;

            //dynamic o = new ExpandoObject();
            //o.Prop = "a property";
        }


    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required]
        [EmailAddress]
        [Display(Name ="E-mail address")]
        [StringLength(100)]
        public string Email { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
