using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RockwellBlog.Models;
using RockwellBlog.Services;

namespace RockwellBlog.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<BlogUser> _userManager;
        private readonly SignInManager<BlogUser> _signInManager;
        private readonly IBlogImageService _blogImageService;

        public IndexModel(
            UserManager<BlogUser> userManager,
            SignInManager<BlogUser> signInManager,
            IBlogImageService blogImageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _blogImageService = blogImageService;
        }

        public string Username { get; set; }

        public string CurrentImage { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Display Name")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string DisplayName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Update Profile Image")]
            public IFormFile NewImage { get; set; }
        }

        private async Task LoadAsync(BlogUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            CurrentImage = _blogImageService.DecodeImage(user.ImageData, user.ContentType);

            Input = new InputModel
            {
                DisplayName = user.DisplayName,
                PhoneNumber = phoneNumber

            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            var hasChanged = false;

            //Store the new displayName if it has changed
            if (user.DisplayName != Input.DisplayName)
            {
                //store the new name
                user.DisplayName = Input.DisplayName;
                hasChanged = true;
            }

            if (Input.NewImage is not null)
            {
                user.ImageData = await  _blogImageService.EncodeFileAsync(Input.NewImage);
                hasChanged = true;

                user.ContentType = _blogImageService.ContentType(Input.NewImage);
                hasChanged = true;
            }

            if (hasChanged == true)
            {
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
