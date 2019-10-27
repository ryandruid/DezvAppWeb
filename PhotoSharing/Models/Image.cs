using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoSharing.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public DateTime Created { get; set; }
        //[Required]
        public string Path { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Category Category { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }
    }

  /*  public class ImageDBContext : DbContext
    {
        public ImageDBContext() : base("DBConnectionString") { }
        public DbSet<Image> Images { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Comment> Comments { get; set; }

    }
    */
}