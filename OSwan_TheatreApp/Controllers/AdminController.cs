﻿using System;
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

        //Post used for searching up a user
        [HttpPost]
        public ActionResult Index(string SearchString)
        {
            //Searching via first name is much more desireable
            var users = db.Users.Where(u => u.FirstName == SearchString.Trim());
            return View(users.ToList());
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

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public ActionResult EditModerator(string id)
        {
            //If id is null then return bad request
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Find the mod by ID
            Moderator moderator = db.Users.Find(id) as Moderator;

            if (moderator == null)
            {
                return HttpNotFound();
            }

            //Send mods details to the view
            return View(new EditModeratorViewModel
            {
                Street = moderator.Street,
                City = moderator.City,
                FirstName = moderator.FirstName,
                LastName = moderator.LastName,
                PhoneNumber = moderator.PhoneNumber,
                PostCode = moderator.PostCode,
                ModType = moderator.ModType,
                IsSuspended = moderator.IsSuspended


            });
        }

        //Post editMod
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditModerator(string id,
            [Bind(Include = "FirstName,LastName,Address,City,PostCode,PhoneNumber,ModType,IsSuspended")] EditModeratorViewModel model)
        {
            if (ModelState.IsValid)
            {
                Moderator mod = (Moderator)await UserManager.FindByIdAsync(id);//Find user by id

                UpdateModel(mod);//Update the new mod details by using the model

                IdentityResult result = await UserManager.UpdateAsync(mod);//update the new mod details on the database

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            return View(model);
        }

        public ActionResult EditRegisteredUser(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Find the user by ID
            RegisteredUser user = db.Users.Find(id) as RegisteredUser; //find user by id and return it

            if (user == null)
            {
                return HttpNotFound();
            }

            //Send users details to the view
            return View(new EditRegisteredUserViewModel
            {
                Street = user.Street,
                City = user.City,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PostCode = user.PostCode,
                TrustedUser = user.TrustedUser,
                IsSuspended = user.IsSuspended
            }); ;
        }

        [HttpPost]
        public async Task<ActionResult> EditRegisteredUser(string id,
           [Bind(Include = "FirstName,LastName,Address,City,PostCode,PhoneNumber,CustomerType,IsSuspended")] EditRegisteredUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                //get user from database by id
                RegisteredUser user = (RegisteredUser)await UserManager.FindByIdAsync(id);

                UpdateModel(user);//Update user details useing the values from the model

                IdentityResult result = await UserManager.UpdateAsync(user); //update user details in the database

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            return View(model); //If failure occurs view and model return to the view
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = db.Users.Find(id); //Finds and stores a User as user via ID
            if (user == null)
            {
                return HttpNotFound();
            }

            //No viewing of Admins details, keeping system secure!
            if (user is RegisteredUser)
            {
                return View("DetailsRegisteredUser", (RegisteredUser)user);
            }

            if (user is Moderator)
            {
                return View("DetailsModerator", (Moderator)user);
            }

            return HttpNotFound();
        }

        //Promotion of roles/demotion
        [HttpGet]
        public async Task<ActionResult> ChangeRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Can't change your own role
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Admin");
            }

            //Get user by id
            User user = await UserManager.FindByIdAsync(id);

            //Get users current role
            string oldRole = (await UserManager.GetRolesAsync(id)).Single(); //Only ever a single role


            //Get all the roles from the database and store them in a list of selecteditems
            var items = db.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == oldRole
            }).ToList();

            //Build the change role view model object including the list of roles
            //Send it to the view displaying thee roles in a dropdown list with user's current role displayed as text
            return View(new ChangeViewModel
            {
                UserName = user.UserName,
                Roles = items,
                OldRole = oldRole,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeRole")]
        public async Task<ActionResult> ChangeRoleConfirmed(string id, [Bind(Include = "Role")] ChangeViewModel model)
        {
            //Can't change your own role
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                User user = await UserManager.FindByIdAsync(id); //get user by id

                //get users old role
                string oldRole = (await UserManager.GetRolesAsync(id)).Single();

                //if the current role is the same as selected role then there is no need to update database
                if (oldRole == model.Role)
                {
                    return RedirectToAction("Index", "Admin");
                }

                //Remove user from old role first
                await UserManager.RemoveFromRoleAsync(id, oldRole);

                //Now we add the user to the new role
                await UserManager.AddToRoleAsync(id, model.Role);

                if (model.Role == "Suspended")
                {
                    //Then set IsSuspended to true
                    user.IsSuspended = true;

                    //Update user's details in the database
                    await UserManager.UpdateAsync(user);
                }

                return RedirectToAction("Index", "Admin");
            }
            return View(model);
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Can't delete your own account
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Admin");
            }

            //find user by id in the database
            User user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            //Delete user
            await UserManager.DeleteAsync(user);

            return RedirectToAction("Index", "Admin");
        }

        //Disposing
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}