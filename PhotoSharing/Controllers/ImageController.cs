using Microsoft.AspNet.Identity;
using PhotoSharing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoSharing.Controllers
{
    public class ImageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Image
        public ActionResult Index()
        {
                       
            var images = db.Images.Include("Category").Include("User");
            
            ViewBag.Images = images;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View();
        }

        
        public ActionResult Show(int id)
        {
            Image image = db.Images.Find(id);
            ViewBag.Image = image;

            var comments = from comment in db.Comments
                         where comment.Id == image.Id
                         orderby comment.Created descending
                         select comment;
            ViewBag.Comments = comments;

            return View(image);
        }

        [Authorize(Roles = "User, Administrator")]
        public ActionResult New()
        {
           Image image = new Image();
            
            // preluam lista de categorii din metoda GetAllCategories()
            image.Categories = GetAllCategories();
            image.UserId = User.Identity.GetUserId();
            return View(image);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();
            // Extragem toate categoriile din baza de date
            var categories = from cat in db.Categories select cat;
            // iteram prin categorii
            foreach (var category in categories)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            // returnam lista de categorii
            return selectList;
        }

        [Authorize(Roles = "User, Administrator")]
        [HttpPost]
        public ActionResult New(Image image)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/Resources/"), fileName);
                            file.SaveAs(path);
                            image.Path = "~/Resources/" + file.FileName;

                        }
                    }
                    db.Images.Add(image);
                    db.SaveChanges();
                    TempData["message"] = "Imaginea a fost adaugata!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(image);
                }
            }
            catch (Exception e)
            {
                return View(image);
            }
        }

        [Authorize(Roles = "User, Administrator")]
        public ActionResult Edit(int id)
        {

            Image image = db.Images.Find(id);
            ViewBag.Image = image;
            image.Categories = GetAllCategories();

            if (image.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                return View(image);
            }
            else
            {
                TempData["message"] = "You have no rights to modify!";
                return RedirectToAction("Show", "Image", new { @id = image.Id });
            }

        }

        [Authorize(Roles = "User, Administrator")]
        [HttpPut]
        public ActionResult Edit(int id, Image requestImage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Image image = db.Images.Find(id);
                    if (TryUpdateModel(image))
                    {
                        image.Title = requestImage.Title;
                        image.Description = requestImage.Description;
                        image.CategoryId = requestImage.CategoryId;
                        db.SaveChanges();
                        TempData["message"] = "Imaginea a fost modificata!";
                    }
                    return RedirectToAction("Show", "Image", new { @id = image.Id });
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


        [Authorize(Roles = "User, Administrator")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {

            Image image = db.Images.Find(id);

            if (image.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {

                string fullPath = Request.MapPath(image.Path);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                db.Images.Remove(image);
                db.SaveChanges();
                TempData["message"] = "Imaginea a fost stearsa!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["message"] = "You don't have the right to delete!";
                return RedirectToAction("Show", "Image", new { @id = image.Id });
            }
        }
    }
}