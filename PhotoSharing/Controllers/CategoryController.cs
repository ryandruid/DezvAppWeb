using PhotoSharing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoSharing.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Category
        [Authorize(Roles = "User, Administrator")]
        public ActionResult Index()
        {
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;

            ViewBag.Categories = categories;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View();
        }

        [Authorize(Roles = "User, Administrator")]
        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Category = category;

            var images = from image in db.Images
                         where image.CategoryId == category.CategoryId
                         orderby image.Created descending
                         select image;
            ViewBag.Images = images;

            

            return View(category);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult New()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult New(Category category)
        {
            try
            {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost adaugata!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Category = category;
            return View(category);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut]
        public ActionResult Edit(int id, Category requestCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = db.Categories.Find(id);
                    if (TryUpdateModel(category))
                    {
                        category.CategoryName = requestCategory.CategoryName;
                        db.SaveChanges();
                        TempData["message"] = "Categoria a fost modificata!";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestCategory);
                }

            }
            catch (Exception e)
            {
                return View(requestCategory);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["message"] = "Categoria a fost stearsa!";
            return RedirectToAction("Index");
        }
    }
}