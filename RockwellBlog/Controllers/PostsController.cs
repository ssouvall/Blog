﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RockwellBlog.Data;
using RockwellBlog.Models;
using RockwellBlog.Services;
using X.PagedList;

namespace RockwellBlog.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogImageService _blogImageService;
        private readonly IConfiguration _configuration;
        private readonly BasicSlugService _slugService;
        private readonly SearchService _searchService;

        public PostsController(ApplicationDbContext context, IBlogImageService blogImageService, IConfiguration configuration, BasicSlugService slugService, SearchService searchService)
        {
            _context = context;
            _blogImageService = blogImageService;
            _configuration = configuration;
            _slugService = slugService;
            _searchService = searchService;
        }

        [AllowAnonymous]
        public async Task<ActionResult> BlogPostIndex(int? id, int? page)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pageNumber = page ?? 1;
            var pageSize = 3;

            var blogPostsFiltered = await _context.Posts.Where(p => p.BlogId == id && p.PublishState == Enums.PublishState.ProductionReady)
                                                .OrderByDescending(b => b.Created)
                                                .ToPagedListAsync(pageNumber, pageSize);

            var blogPosts = await _context.Posts.Where(p => p.BlogId == id)
                                                .OrderByDescending(b => b.Created)
                                                .ToPagedListAsync(pageNumber, pageSize);
            //specify which view to redirect to, otherwise it will redirect to BlogPostIndex which doesn't exist

            var blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
            ViewData["HeaderText"] = blog.Name;
            ViewData["SubText"] = blog.Description;
            ViewData["HeaderImage"] = _blogImageService.DecodeImage(blog.ImageData, blog.ContentType);

            if(User.IsInRole("Administrator") || User.IsInRole("Moderator"))
            {
                return View(blogPosts);
            }
            else
            {
                return View(blogPostsFiltered);
            }
            
        }

        // GET: Posts
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Blog);
            return View(await applicationDbContext.ToListAsync());
        }

        [AllowAnonymous]
        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (post == null)
            {
                return NotFound();
            }

            ViewData["HeaderText"] = post.Title;
            ViewData["SubText"] = post.Abstract;
            ViewData["HeaderImage"] = _blogImageService.DecodeImage(post.ImageData, post.ContentType);
            ViewData["AuthorText"] = $"Created by Stephen Souvall on {post.Created.ToString("MMM dd, yyyy")}";
            //ViewData["ModerationType"] = new SelectList(_context.Comment, "Id", "ModerationType");



            return View(post);
        }

        [AllowAnonymous]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchIndex(int? page, string searchString)
        {
            ViewData["SearchString"] = searchString;
            //Step 1: I need a set of results stemming from this searchString

            var posts = _searchService.SearchContent(searchString);

            var pageNumber = page ?? 1;
            var pageSize = 6;
            
            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }


        // GET: Posts/Create
        public IActionResult Create(int? id)
        {
            if (id is not null)
            {
                ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", id);
            }
            else
            {
                ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            }

            ViewData["HeaderImage"] = "//i.ibb.co/Y00pYxH/Banner.png";
            ViewData["HeaderText"] = "Let's Create a Post.";
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,PublishState,ImageFile,IsFeatured")] Post post)
        {
            if (ModelState.IsValid)
            {
                //created date
                post.Created = DateTime.Now;

                //upload blog image. If there is no image, default to defaultBlogImage
                post.ImageData = await _blogImageService.EncodeFileAsync(post.ImageFile) ??
                    await _blogImageService.EncodeFileAsync(_configuration["DefaultPostImage"]);

                post.ContentType = post.ImageFile is null ?
                        _configuration["DefaultPostImage"].Split('.')[1] :
                        _blogImageService.ContentType(post.ImageFile);

                //Slug stuff goes here
                var slug = _slugService.UrlFriendly(post.Title);
                if(!_slugService.IsUnique(slug))
                {
                    //I must now add a Model Error and inform the user of the problem
                    ModelState.AddModelError("Title", "There is an issue with the title, please try again");
                    return View(post);
                }

                post.Slug = slug;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("BlogPostIndex", new { id = post.BlogId});
            }
            
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View("BlogPostIndex", post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["HeaderImage"] = _blogImageService.DecodeImage(post.ImageData, post.ContentType);
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,Created,Slug,PublishState,ImageData,ImageFile,ContentType,IsFeatured")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newSlug = _slugService.UrlFriendly(post.Title);
                    //Need to compare original slug with current slug
                    if(post.Slug != newSlug)
                    {
                        if(!_slugService.IsUnique(newSlug))
                        {
                            ModelState.AddModelError("Title", "There is an issue with the title, please try again");
                            return View(post);
                        }

                        post.Slug = newSlug;
                    }

                    if (post.ImageFile is not null)
                    {
                        post.ImageData = await _blogImageService.EncodeFileAsync(post.ImageFile);
                        post.ContentType = _blogImageService.ContentType(post.ImageFile);
                    }

                    post.Updated = DateTime.Now;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Posts", new { post.Slug });
            }


            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .ThenInclude(b => b.Id)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
