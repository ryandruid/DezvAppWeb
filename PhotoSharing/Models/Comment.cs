using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhotoSharing.Models
{
    public class Comment
    {
        [Key]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Cannot create empty comment")]
        public string Content { get; set; }
        [Required]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User1 { get; set; }

        /// FK restrictions
        public virtual Image Image { get; set; }
        
    }
}