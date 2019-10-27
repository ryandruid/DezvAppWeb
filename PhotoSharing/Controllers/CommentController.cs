using Microsoft.AspNet.Identity;
using PhotoSharing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PhotoSharing.Controllers
{
    [Authorize(Roles = "User, Administrator")]
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Comment
        public ActionResult New(int? id)
        {
            Comment comment = new Comment();
            comment.Image = db.Images.Find(id);
            comment.UserId = User.Identity.GetUserId();

            return View(comment);
        }

       
        [HttpPost]
        public ActionResult New(Comment comment)
        {

            try
            {
                if (ModelState.IsValid)
                {

                    db.Comments.Add(comment);
                    db.SaveChanges();
                    //TempData["message"] = "Post added!";
                    return RedirectToAction("Show", "Image", new { @id = comment.Id });

                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {

            Comment comment = db.Comments.Find(id);
            ViewBag.Comment = comment;

            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                return View(comment);
            }
            else
            {
                TempData["message"] = "You have no rights to modify!";
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPut]
        public ActionResult Edit(int id, Comment requestComment)
        {


            try
            {
                
                if (ModelState.IsValid)
                {
                    Comment comment = db.Comments.Find(id);

                    if (TryUpdateModel(comment))
                    {
                        
                        comment.Content = requestComment.Content;
                        db.SaveChanges();
                        TempData["message"] = "Comment modified!";
                    }
                    return RedirectToAction("Show", "Image", new { @id = comment.Id });
                }
                else
                {
                    return View(requestComment);
                }

            }
            catch (Exception e)
            {
                return View(requestComment);
            }
        }

       [HttpDelete]
        public ActionResult Delete(int id)
        {
            
                Comment comment = db.Comments.Find(id);
            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                db.Comments.Remove(comment);
                db.SaveChanges();


                return RedirectToAction("Show", "Image", new { @id = comment.Id });
            }
            else
            {
                TempData["message"] = "You don't have the right to delete!";
                return RedirectToAction("Index", "Home");
            }
        }
        

    }
}