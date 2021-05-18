using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RockwellBlog.Data;
using RockwellBlog.Models;
using RockwellBlog.Services;

namespace RockwellBlog.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogImageService _blogImageService;
        private readonly IConfiguration _configuration;

        public BlogsController(ApplicationDbContext context, IBlogImageService blogImageService, IConfiguration configuration)
        {
            _context = context;
            _blogImageService = blogImageService;
            _configuration = configuration;
        }

        // GET: Blogs
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.ToListAsync());
        }

        // GET: Blogs/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            ViewData["HeaderImage"] = "//i.ibb.co/Y00pYxH/Banner.png";
            ViewData["HeaderText"] = "Create a New Category";
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ImageFile")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                //created date
                blog.Created = DateTime.Now;

                //upload blog image. If there is no image, default to defaultBlogImage
                blog.ImageData = await _blogImageService.EncodeFileAsync(blog.ImageFile) ??
                    await _blogImageService.EncodeFileAsync(_configuration["DefaultBlogImage"]);
                
                blog.ContentType = blog.ImageFile is null ?
                        _configuration["DefaultBlogImage"].Split('.')[1] :
                        _blogImageService.ContentType(blog.ImageFile);

                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(blog);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Created,Updated,ImageData,ContentType")] Blog blog, IFormFile NewBlogImage)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Update date
                    blog.Updated = DateTime.Now;

                    if (NewBlogImage is not null)
                    {
                        blog.ContentType = _blogImageService.ContentType(NewBlogImage);
                        blog.ImageData = await _blogImageService.EncodeFileAsync(NewBlogImage);
                    }

                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
