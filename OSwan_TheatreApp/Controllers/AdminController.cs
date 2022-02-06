using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OSwan_TheatreApp.Models;
using OSwan_TheatreApp.Models.ViewModels;


namespace OSwan_TheatreApp.Controllers
{
    //Inherits from AccountController for login/registration functionality
    [Authorize(Roles ="Admin")]//Only admins can access this controller
    public class AdminController : AccountController
    {

        //Instance of Db Context
        //This can now be used throughout the class
        private TheatreDbContext db = new TheatreDbContext();

        public AdminController() : base()
        {

        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager)
        {

        }


        // GET: Admin
        public ActionResult Index()
        {
            //Get all the users and order them by registration date
            var users = db.Users.OrderBy(u => u.RegisteredAt).ToList();

            //Return the list of users to the index view
            return View(users);
        }

        //Get
        [HttpGet]
        public ActionResult CreateRegisteredUser()
        {
            //Instance of view model class
            CreateUserViewModel user = new CreateUserViewModel();

            //Retrieving all roles from the database and storing them into a selected list item so the roles can be displayed in a dropdownlist
            var roles = db.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name

            }).ToList();

            //assign roles to the user roles prop
            user.Roles = roles;

            //Send user model to the view
            return View(user);
        }

        //Post
        [HttpPost]
        public ActionResult CreateRegisteredUser(CreateUserViewModel model)
        {
            //if the model state is not null
            if (ModelState.IsValid)
            {
                RegisteredUser newUser = new RegisteredUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    Street = model.Street,
                    City = model.City,
                    PostCode = model.PostCode,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    TrustedUser = model.TrustedUser,
                    IsSuspended = false,
                    RegisteredAt = DateTime.Now
                };

                //Create user and store it into DB
                //Password will also be hashed
                var result = UserManager.Create(newUser, model.Password);

                //If user was stored in DB successfully
                if (result.Succeeded)
                {
                    //add user to the role selected from model
                    UserManager.AddToRole(newUser.Id, model.Role);

                    return RedirectToAction("Index", "Admin");
                }
            }
            //Something went wrong so returned back to user list
            return View(model);
        }
    }
}