using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OSwan_TheatreApp.Models;

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
    }
}