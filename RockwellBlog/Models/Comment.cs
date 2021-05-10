using RockwellBlog.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockwellBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        
        //record foreign key of user
        public string AuthorId { get; set; }
        public string ModeratorId { get; set; }

        public string Body { get; set; }
        public DateTime Created { get; set; }

        //------creating responsibilities/privileges of the moderator------//
        
        //has it been moderated?
        public DateTime? Moderated { get; set; }
        //body of comment after change that is being displayed
        public string ModeratedBody { get; set; }
        //list of reasons for moderator to change things
        public ModerationType? ModerationType { get; set; }
        

        //------//

        public virtual Post Post { get; set; }

        //can name instance of BlogUser Author, doesn't have to be BlogUser. This makes sense since there will be multiple instances of this.
        public virtual BlogUser Author { get; set; }
        public virtual BlogUser Moderator { get; set; }
    }
}
