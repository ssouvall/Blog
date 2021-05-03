using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockwellBlog.Models
{
    public class BlogUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        //public byte[] ImageData { get; set; }
        //public string ContentType { get; set; }
        //ContentType is a property of IFormFile, which carries files transmitted over HTTP requests.Gives access to metadata related to the file
    }
}
