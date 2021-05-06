using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RockwellBlog.Models
{
    public class Blog
    {
        public int Id { get; set; }
        //public string AuthorId { get; set; }
        //would have this if you were allowing multiple people to create posts. Won't have it because I'm the only one creating posts.
        //the numbers in the string below are indexes of properties
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at last {2} and at most {1} characters.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at last {2} and at most {1} characters.", MinimumLength = 2)]
        public string Description { get; set; }

        //takes out the time aspect and formats the date
        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }
        //would make most sense to set this up programmatically in an http post rather than letting the user set this up
        //same with date updated
        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        [Display(Name = "Upload Image")]
        public byte[] ImageData { get; set; }

        public string ContentType { get; set; }

        [NotMapped]
        [Display(Name="Choose Blog Image")]
        public IFormFile ImageFile { get; set; }

        //Navigational properties. This is the property that stores collection of blog posts
        //The virtual works with entity framework, allows "lazy loading" of data. Has to do with when related data is retrieved from the database. 
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();

    }
}
