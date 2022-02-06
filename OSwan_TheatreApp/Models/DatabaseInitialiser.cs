using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OSwan_TheatreApp.Models
{
    //Using Drop Create Always stratagey
    public class DatabaseInitialiser : DropCreateDatabaseAlways<TheatreDbContext>
    {
        //Seeding DB
        protected override void Seed(TheatreDbContext context)
        {
            base.Seed(context);

            //if there are no records in User table
            if (!context.Users.Any())
            {
                //Rolemanager initialized
                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                //if Admin role does not exist
                if (!roleManager.RoleExists("Admin"))
                {
                    //Admin role created
                    roleManager.Create(new IdentityRole("Admin"));
                }

                //if Moderator role does not exist
                if (!roleManager.RoleExists("Moderator"))
                {
                    //Admin role created
                    roleManager.Create(new IdentityRole("Moderator"));
                }

                //if RegisteredUser role does not exist
                if (!roleManager.RoleExists("RegisteredUser"))
                {
                    //Admin role created
                    roleManager.Create(new IdentityRole("RegisteredUser"));
                }

                //if Suspended role does not exist
                //This role will be used to discipline users who post foul content
                if (!roleManager.RoleExists("Suspended"))
                {
                    //Admin role created
                    roleManager.Create(new IdentityRole("Suspended"));
                }

                //Saving changes to DB
                context.SaveChanges();

                //UserManager initialized
                UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));

                //Creating an admin
                //First check there is no admin user already existing
                if (userManager.FindByName("adminolly@theatre.com") == null)
                {
                    //Password Validator boundries
                    userManager.PasswordValidator = new PasswordValidator
                    {
                        RequireDigit = false,
                        RequiredLength = 1,
                        RequireLowercase = false,
                        RequireNonLetterOrDigit = false,
                        RequireUppercase = false
                    };

                    //Create admin user
                    var administrator = new Admin
                    {
                        UserName = "adminolly@theatre.com",
                        Email = "adminolly@theatre.com",
                        FirstName = "Olly",
                        LastName = "Swan",
                        Street = "Made up Street",
                        City = "Glasgow",
                        PostCode = "N01 24W",
                        RegisteredAt = DateTime.Now.AddYears(-5),
                        EmailConfirmed = true,
                        IsSuspended = false,
                        AdminStatus = AdminStatus.FullTime
                        
                    };

                    //Adding new admin to users table
                    userManager.Create(administrator, "admin123");
                    //Assign to admin role
                    userManager.AddToRole(administrator.Id, "Admin");
                }

                //Creating a moderator
                //First check there is no moderator user already existing
                if (userManager.FindByName("millie@hotmail.com") == null)
                {
                    //Create moderator user
                    var moderator = new Moderator
                    {
                        UserName = "millie@hotmail.com",
                        Email = "millie@hotmail.com",
                        FirstName = "Millie",
                        LastName = "Stewart",
                        Street = "Made up Street",
                        City = "Ayr",
                        PostCode = "218 HDA",
                        RegisteredAt = DateTime.Now.AddYears(-3),
                        EmailConfirmed = true,
                        IsSuspended = false,
                        ModType = ModType.Jr
                    };

                    //Adding new admin to users table
                    userManager.Create(moderator, "mod123");
                    //Assign to admin role
                    userManager.AddToRole(moderator.Id, "Moderator");
                }

                //Creating a RegisteredUser
                //First check there is no RegisteredUser user already existing
                if (userManager.FindByName("daisy@hotmail.com") == null)
                {
                    //Create Registered user
                    var registeredUser = new RegisteredUser
                    {
                        UserName = "daisy@hotmail.com",
                        Email = "daisy@hotmail.com",
                        FirstName = "Daisy",
                        LastName = "Currie",
                        Street = "Made up Street",
                        City = "Prestwick",
                        PostCode = "927 HSJH",
                        RegisteredAt = DateTime.Now.AddYears(-1),
                        EmailConfirmed = true,
                        IsSuspended = false,
                        TrustedUser = TrustedUser.Trusted
                        
                    };

                    //Adding new admin to users table
                    userManager.Create(registeredUser, "Password123");
                    //Assign to admin role
                    userManager.AddToRole(registeredUser.Id, "RegisteredUser");
                }

                //Creating a Suspended user
                //First check there is no suspended user already existing
                if (userManager.FindByName("dom@hotmail.com") == null)
                {
                    //Create Registered user
                    var registeredUser = new RegisteredUser
                    {
                        UserName = "dom@hotmail.com",
                        Email = "dom@hotmail.com",
                        FirstName = "Dom",
                        LastName = "Swan",
                        Street = "Made up Street",
                        City = "Belmont",
                        PostCode = "Ka7 2J5",
                        RegisteredAt = DateTime.Now.AddYears(-2),
                        EmailConfirmed = true,
                        IsSuspended = false
                    };

                    //Adding new admin to users table
                    userManager.Create(registeredUser, "Password123");
                    //Assign to admin role
                    userManager.AddToRole(registeredUser.Id, "Suspended");
                }

                //saving changes to DB
                context.SaveChanges();
            }
        }
    }
}