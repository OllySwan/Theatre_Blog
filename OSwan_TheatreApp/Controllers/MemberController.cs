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

       
    }
}