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
using System.IO;

namespace OSwan_TheatreApp.Controllers
{
    public class MemberController : Controller
    {
        private TheatreDbContext context = new TheatreDbContext();

        // GET: Member
        public ActionResult CreatePost()

        {
            if(User.IsInRole("Suspended"))
            {
                RedirectToAction("Suspended", "Home");
            }

            ViewBag.CategoryId = new SelectList(context.Categories, "CategoryId", "Name");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost([Bind(Include = "PostId, UserId, Title, User ,MainBody, DatePublished, CategoryId")] Post post, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                //Setting the date posted to current time of creation
                post.DatePosted = DateTime.Now;

                //Setting approval status to TBC (admin will have to approve)
                post.ApprovalStatus = ApprovalStatus.TBC;

                //Assigning userID to post
                post.UserId = User.Identity.GetUserId();

                if(file != null)
                {
                    //Not using a viewmodel here so had trouble with implementation of "postID"
                    //To improvise and keep the filename unique I used CategoryID + Post title 
                    int dotPosition = Path.GetFileName(file.FileName).IndexOf(".");

                    string fileExtension = Path.GetFileName(file.FileName).Substring(dotPosition);

                    //Keeps image file name unique
                    var fileName = post.CategoryId + post.Title + fileExtension;

                    //Setting path
                    var path = Path.Combine(Server.MapPath("~/Images/Uploaded"), fileName);
                    file.SaveAs(path);

                    //Assigning imageurl, this helps with the front end 
                    //Let's front end know if there is an image to be showed or not.
                    post.ImageUrl = path;
                }

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

        public ActionResult DeletePost(int? id)
        {
            //if id is null then return bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Find post that has been passed
            Post post = context.Posts.Find(id);

            //Find posts category
            var category = context.Categories.Find(post.CategoryId);

            //assigning navigational properties
            post.Category = category;

            //if post is null, throw error
            if (post == null)
            {
                return HttpNotFound();
            }

            //return delete view and send post
            return View(post);
        }

        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Find post by id
            Post post = context.Posts.Find(id);

            //Remove post from DB
            context.Posts.Remove(post);

            //Storing comments from post ID and removing them when post is removed (Cascading delete)
            var comments = context.Comments.Where(c => c.CommentId == post.PostId).ToList();
            context.Comments.RemoveRange(comments);

            //Save changes in DB
            context.SaveChanges();

            //tempdata for message
            TempData["AlertMessage"] = "Are you sure you want to delete this post?";

            //Redirect to all posts view
            return RedirectToAction("UsersPosts");
        }

        [HttpGet]
        public ActionResult CreateComment()
        {

            PostCommentViewModel postCommentVM = new PostCommentViewModel();



            return View(postCommentVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment(PostCommentViewModel model)
        {
            //Error checking by checking model that has been passed is valid
            if (ModelState.IsValid)
            {
                //storing post finding/using modelId
                var post = context.Posts.Find(model.Id);

                //Constructing a comment
                var comment = new Comment();

                comment.Post = post;
                comment.PostId = post.PostId;
                comment.UserId = post.UserId;
                comment.User = post.User;
                comment.Text = model.Text;
                comment.CommentAuthor = model.CommentAuthor;
                comment.Date = DateTime.Now;
                comment.Post = post;
                comment.commentApprovalStatus = commentApprovalStatus.TBC;

                //Add comment to comments table
                context.Comments.Add(comment);

                //Storing userID
                var userID = User.Identity.GetUserId();

                //Finding user by ID
                var user = context.Users.Find(userID);

                //Adding comment to users comment list
                user.Comments.Add(comment);

                //Saving changes to DB
                context.SaveChanges();

                return RedirectToAction("BlogHome", "Home");

            }
            return View(model);
        }

    }
}