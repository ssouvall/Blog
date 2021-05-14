using RockwellBlog.Data;
using RockwellBlog.Models;
using RockwellBlog.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockwellBlog.Services
{
    public class SearchService
    {
        //give ability to talk to the database
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IOrderedQueryable<Post> SearchContent(string searchString)
        {
            //Step 1: Get an IQueryable that contains all the posts in the event that 
            // the user does not supply a search string
            var result = _context.Posts.Where(p => p.PublishState == PublishState.ProductionReady);

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                result = result.Where(p => p.Title.ToLower().Contains(searchString) ||
                                            p.Abstract.ToLower().Contains(searchString) ||
                                            p.Content.ToLower().Contains(searchString) ||
                                            p.Comments.Any(c => c.Moderated == null &&
                                                                c.Body.ToLower().Contains(searchString) ||
                                                                c.ModeratedBody.ToLower().Contains(searchString) ||
                                                                c.Author.FirstName.ToLower().Contains(searchString) ||
                                                                c.Author.LastName.ToLower().Contains(searchString) ||
                                                                c.Author.Email.ToLower().Contains(searchString)));

            }

            return result.OrderByDescending(p => p.Created);
        
        }
    }
}
