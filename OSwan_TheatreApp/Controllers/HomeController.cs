using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OSwan_TheatreApp.Models;
using OSwan_TheatreApp.Models.ViewModels;

namespace OSwan_TheatreApp.Controllers
{
    
    public class HomeController : Controller
    {
        //Instance of db context
        private TheatreDbContext context = new TheatreDbContext();

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult BlogHome()
        {
            //Retrieving and storing all posts whilst inclduing some properties
            //Order all posts from date posted
            var posts = context.Posts.Include(p => p.Category).Include(p => p.User).OrderByDescending(p => p.DatePosted);

            //Sending list of categories to view
            ViewBag.Categories = context.Categories.ToList();

            //Sending posts in a list to view
            return View(posts.ToList());
        }

        public ActionResult Details(int id)
        {
            //Search for desired post in db
            Post post = context.Posts.Find(id);

            //Finding and storing the user who created the post
            var user = context.Users.Find(post.UserId);

            //Finding the category the post belongs to
            var category = context.Categories.Find(post.CategoryId);

            //assign user to post
            post.User = user;

            //assign category to post
            post.Category = category;


            //send post model to view
            return View(post);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult CreateComment()
        {
            PostCommentViewModel postCommentVM = new PostCommentViewModel();

            

            return View(postCommentVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment( PostCommentViewModel model)
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
                
                //Add comment to comments table
                context.Comments.Add(comment);

                //Saving changes to DB
                context.SaveChanges();

                return RedirectToAction("BlogHome", "Home");

            }
            return View(model);
        }


        // GET: Mod
        [Authorize(Roles = "Moderator, Admin")]//Only admins & Moderators can access this action
        [HttpGet]
        public ActionResult DeleteComment(int? id)
        {
            //if id is null then return bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Find post that has been passed
            Comment comment = context.Comments.Find(id);

            //if comment is null, throw error
            if (comment == null)
            {
                return HttpNotFound();
            }

            //return delete view and send comment
            return View(comment);
        }

        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(int id)
        {
            //Find comment by id
            Comment comment = context.Comments.Find(id);

            //Remove comment from DB
            context.Comments.Remove(comment);

            //Save changes in DB
            context.SaveChanges();

            //Redirect to all posts view
            return RedirectToAction("BlogHome");
        }

        public ActionResult AnnouncementOfTheDay()
        {
            //storing all posts that have a category of announcment and ordering them by date
            var announcements = context.Posts.Where(p => p.CategoryId == 2).OrderByDescending(p => p.DatePosted);

            //Storing latest announcement to send to view
            var announcement = announcements.FirstOrDefault();

            //Sending latest announcement to view
            return View(announcement);
        }

    }
}