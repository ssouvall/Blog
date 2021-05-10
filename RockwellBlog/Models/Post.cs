using Microsoft.AspNetCore.Http;
using RockwellBlog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RockwellBlog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int BlogId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at last {2} and at most {1} characters.", MinimumLength = 2)]
        public string Title { get; set; }
        
        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at last {2} and at most {1} characters.", MinimumLength = 2)]
        public string Abstract { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime? Updated { get; set; }

        [Display(Name = "Upload Image")]
        public byte[] ImageData { get; set; }

        public string ContentType { get; set; }

        [NotMapped]
        [Display(Name = "Choose Blog Image")]
        public IFormFile ImageFile { get; set; }


        public string Slug { get; set; }
        //need a way to record the state of a post (whether it's ready to be published/viewed). Maybe have preview mode, etc. Will use an enum for this.
        //access enum PublishState. Will create a dropdown with multiple options. Enum lives on its own and is a tool that can be used elsewhere.
        //by default in the database the enum stores the numerical value of the dropdown. Indexed from 0.
        
        public PublishState PublishState { get; set; }

        //navigational property for the child class
        //referenced by the BlogId. Creates new instance of blog.
        //brings in the entire blog record represented by the BlogId reference
        public virtual Blog Blog { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
