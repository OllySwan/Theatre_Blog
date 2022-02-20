using Microsoft.AspNet.Identity;
using OSwan_TheatreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Net;
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

        public ActionResult Details(int? id)
        {
            //Error checking
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Finding post with the passed ID
            Post post = context.Posts.Find(id);

            //Further error checking
            if (post == null)
            {
                return HttpNotFound();
            }

            //Send post to view
            return View(post);
        }

        public ActionResult Edit(int? id)
        {
            //Ensuring the ID is not null
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Finding post by ID
            Post post = context.Posts.Find(id);

            User user = context.Users.Find(post.UserId);



            //Further error catching
            if (post == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryId = new SelectList(context.Categories, "CategoryId", "Name", post.CategoryId);


            //Send post to view
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId, Title, MainBody, UserId, CategoryId,ApprovalStatus")] Post post)
        {
            //finding and storing original copy of post
            Post dbPost = context.Posts.Find(post.PostId);

            if (ModelState.IsValid)
            {
                //Approval status updated
                dbPost.ApprovalStatus = ApprovalStatus.Approved;

                //Updating post title
                dbPost.Title = post.Title;

                //Setting new main body
                dbPost.MainBody = post.MainBody;

                //Setting new categoryID
                dbPost.CategoryId = post.CategoryId;

                //Updating date on post
                dbPost.DatePosted = DateTime.Now;

                //Update posts state to modified
                context.Entry(dbPost).State = EntityState.Modified;

                //Save changes to DB
                context.SaveChanges();

                return RedirectToAction("UsersPosts");
            }

            return View(post);
        }

    }
}