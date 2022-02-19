using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace OSwan_TheatreApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public abstract class User : IdentityUser
    {
        //Extending Identity user with following properties
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Display(Name ="Last Name")]
        public string LastName { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        [Display(Name ="Post Code")]
        public string PostCode { get; set; }

        [Display(Name ="Joined")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime RegisteredAt { get; set; }

        [Display(Name ="Suspended")]
        public bool IsSuspended { get; set; }

        //Navigational property Comments
        public virtual List<Post> Posts { get; set; } //Reperesents the many

        //Navigational property Comments
        public virtual List<Comment> Comments { get; set; } //Reperesents the many

        //User Role allocation
        private ApplicationUserManager userManager;

        [NotMapped]
        public string CurrentRole
        {
            get
            {
                if(userManager == null)
                {
                    userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }

                return userManager.GetRoles(Id).Single();
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}