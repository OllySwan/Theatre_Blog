using Microsoft.AspNet.Identity;
using OSwan_TheatreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Security;
using OSwan_TheatreApp.Models.ViewModels;
using Microsoft.AspNet.Identity.Owin;

namespace OSwan_TheatreApp.Controllers
{
    public class MemberController : Controller
    {
        private TheatreDbContext context = new TheatreDbContext();

        // GET: Member
        public ActionResult CreatePost()

        {
            ViewBag.CategoryId = new SelectList(context.Categories, "CategoryId", "Name");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost([Bind(Include = "PostId, UserId, Title, User ,MainBody, DatePublished, CategoryId")] Post post)
        {
            if (ModelState.IsValid)
            {
                //Setting the date posted to current time of creation
                post.DatePosted = DateTime.Now;

                //Setting approval status to TBC (admin will have to approve)
                post.ApprovalStatus = ApprovalStatus.TBC;

                //Assigning userID to post
                post.UserId = User.Identity.GetUserId();

                //Adding created post to table
                context.Posts.Add(post);

                //Saving changes to DB
                context.SaveChanges();

                return RedirectToAction("BlogHome", "Home");
            }

            ViewBag.CategoryId = new SelectList(context.Categories, "CategoryId", "Name", post.CategoryId);

            return View(post);
        }

        public ActionResult UserProfile()
        {
            //If you open app already signed in USER will be null
            //Fresh log in for this to work.

            //Store current users ID 
            string currentUserID = User.Identity.GetUserId();

            //implement user manager functionality
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //Finding user via ID
            User user = userManager.FindById(currentUserID);

            //This solved errors with USER being null
            user.Id = currentUserID;

            return View(user);
        }

        public ActionResult UsersPosts()
        {
            //Get all posts from table
            //Including FK's
            var posts = context.Posts.Include(p => p.Category).Include(p => p.User);

            //Storing userID
            var userID = User.Identity.GetUserId();

            //Narrowing down posts that will be sent to view
            //Only the desired users posts will be sent
            posts = posts.Where(p => p.UserId == userID);

            return View(posts.ToList());
        }

    }
}