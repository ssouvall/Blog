using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RockwellBlog.Data;
using RockwellBlog.Enums;
using RockwellBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockwellBlog.Services
{
    public class DataService
    {
        //constructor injection of ApplicationDbContext
        private readonly ApplicationDbContext _context;
        //make roleManager available
        private readonly RoleManager<IdentityRole> _roleManager;
        //bring in the image service
        private readonly IBlogImageService _blogImageService;
        //bring in UserManager
        private readonly UserManager<BlogUser> _userManager;
        //bring in IConfiguration for managing the password
        private readonly IConfiguration _configuration;

        public DataService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, IBlogImageService blogImageService, UserManager<BlogUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _blogImageService = blogImageService;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task ManageDataAsync()
        {

            //Task 0: Make sure the DB is present by running the migrations
            await _context.Database.MigrateAsync();

            //Task 1: Seed Roles - Creating roles and entering them into the system (AspNetRoles)
            await SeedRolesAsync();

            //Task 2: Seed a few users in the system (AspNetUsers)
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            //Are there any roles already in the system?
            //This will run every time the app runs
            if (_context.Roles.Any())
            {
                return;
            }

            //Spin through an enum and do stuff
            foreach(var role in Enum.GetNames(typeof(BlogRole)))
            {
                //create a role in the system for each role
                await _roleManager.CreateAsync(new IdentityRole(role));
            }


        }

        private async Task SeedUsersAsync()
        {
            //Are there any users already in the system?
            if (_context.Users.Any())
            {
                return;
            }

            //Create a new instance of BlogUser
            var adminUser = new BlogUser()
            {
                Email = "stephensouvall@mailinator.com",
                UserName = "stephensouvall@mailinator.com",
                FirstName = "Stephen",
                LastName = "Souvall",
                DisplayName = "Stephen",
                PhoneNumber = "801-555-1212",
                EmailConfirmed = true,
                ImageData = await _blogImageService.EncodeFileAsync("Me.jpg"),
                ContentType = "jpg"
            };

            //Now need to tap into the user manager and pass it our instance of BlogUser, and reference "AdminPassword" in the appsettings.json
            await _userManager.CreateAsync(adminUser, _configuration["AdminPassword"]);
            //Add the user to a role
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            var moderatorUser = new BlogUser()
            {
                Email = "someone@mailinator.com",
                UserName = "someone@mailinator.com",
                FirstName = "Moderator",
                LastName = "Jones",
                DisplayName = "Moderator",
                PhoneNumber = "801-555-1313",
                EmailConfirmed = true,
                ImageData = await _blogImageService.EncodeFileAsync("Me.jpg"),
                ContentType = "jpg"
            };

            //Now need to tap into the user manager and pass it our instance of BlogUser, and reference "AdminPassword" in the appsettings.json
            await _userManager.CreateAsync(moderatorUser, _configuration["AdminPassword"]);
            //Add the user to a role
            await _userManager.AddToRoleAsync(moderatorUser, BlogRole.Administrator.ToString());

        }
    }   

}
