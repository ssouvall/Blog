using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RockwellBlog.Data;
using RockwellBlog.Models;
using RockwellBlog.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace RockwellBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogImageService _blogImageService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogImageService blogImageService)
        {
            _logger = logger;
            _context = context;
            _blogImageService = blogImageService;
        }

        //update image action to take in page number
        public async Task<IActionResult> Index(int? page)
        {
            
            var lpImageData = await _blogImageService.EncodeFileAsync("Banner.png");
            ViewData["HeaderImage"] = _blogImageService.DecodeImage(lpImageData, "png");
            ViewData["HeaderText"] = "Hi, I'm Stephen Souvall.";
            ViewData["SubText"] = "This is my blog.";

            //Load the view with all blog data
            //must inject ApplicationDbContext _context first then create an instance of it in HomeController


            var pageNumber = page ?? 1;
            var pageSize = 6;

            var allBlogs = await _context.Blogs.OrderByDescending(b => b.Created)
                                                .Include(b => b.Posts)
                                                .ToPagedListAsync(pageNumber, pageSize);

            return View(allBlogs); 
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
