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
    /// <summary>
    /// The model for blog posts
    /// </summary>
    public class Post
    {
        /// <summary>
        /// The Primary Key of the Post
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The foreign key to the blog that is the parent of this post
        /// </summary>
        public int BlogId { get; set; }

        /// <summary>
        /// The title of the post
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at last {2} and at most {1} characters.", MinimumLength = 2)]
        public string Title { get; set; }
        
        /// <summary>
        /// A brief intro to the post to summarize the post content
        /// </summary>
        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at last {2} and at most {1} characters.", MinimumLength = 2)]
        public string Abstract { get; set; }
        
        /// <summary>
        /// The main content of the post
        /// </summary>
        [Required]
        public string Content { get; set; }
        
        /// <summary>
        /// The date the post was created
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }
        
        /// <summary>
        /// The date the post was most recently updated, if applicable
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime? Updated { get; set; }

        /// <summary>
        /// This is the byte array holding the image
        /// </summary>
        [Display(Name = "Upload Image")]
        public byte[] ImageData { get; set; }

        /// <summary>
        /// The file type of the image
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The actual image not stored as byte array
        /// </summary>
        [NotMapped]
        [Display(Name = "Choose Blog Image")]
        public IFormFile ImageFile { get; set; }

        /// <summary>
        /// This is an internal field used to route to the Details action
        /// </summary>
        public string Slug { get; set; }
        //need a way to record the state of a post (whether it's ready to be published/viewed). Maybe have preview mode, etc. Will use an enum for this.
        //access enum PublishState. Will create a dropdown with multiple options. Enum lives on its own and is a tool that can be used elsewhere.
        //by default in the database the enum stores the numerical value of the dropdown. Indexed from 0.
        
        /// <summary>
        /// The current status of the post on the publishing journey
        /// </summary>
        public PublishState PublishState { get; set; }

        /// <summary>
        /// Is the post featured? If marked true it will appear in the home page featured posts carousel
        /// </summary>
        [Display(Name = "Make Featured Post?")]
        public bool IsFeatured { get; set; }


        //navigational property for the child class
        //referenced by the BlogId. Creates new instance of blog.
        //brings in the entire blog record represented by the BlogId reference

        /// <summary>
        /// Navigational property for parent blog
        /// </summary>
        public virtual Blog Blog { get; set; }
        /// <summary>
        /// Navigational property for child comments
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
