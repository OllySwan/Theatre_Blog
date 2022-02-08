using Microsoft.AspNet.Identity;
using OSwan_TheatreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OSwan_TheatreApp.Models.ViewModels;


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
        public ActionResult CreatePost([Bind(Include ="PostId, UserId, Title, User ,MainBody, DatePublished, CategoryId")]Post post)
        {
            if (ModelState.IsValid)
            {
                post.DatePosted = DateTime.Now;

                post.UserId = User.Identity.GetUserId();

                context.Posts.Add(post);

                context.SaveChanges();

                return RedirectToAction("BlogHome", "Home");
            }

            ViewBag.CategoryId = new SelectList(context.Categories, "CategoryId", "Name", post.CategoryId);

            return View(post);
        }
        
    }
}