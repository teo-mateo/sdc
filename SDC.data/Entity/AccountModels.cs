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

        [NotMapped]
        public string Role { get; set; }

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
