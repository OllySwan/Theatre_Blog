using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OSwan_TheatreApp.Models
{
    public class TheatreDbContext : IdentityDbContext<User>
    {

        //Creating tables in DB
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Post> Posts { get; set; }

        public TheatreDbContext()
                : base("TheatreConnection", throwIfV1Schema: false)
            {
            Database.SetInitializer(new DatabaseInitialiser());
            }

            public static TheatreDbContext Create()
            {
                return new TheatreDbContext();
            }

    }
}