using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FromLocalsToLocals.Database;
using FromLocalsToLocals.Models;
using FromLocalsToLocals.ViewModels;
using Geocoding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NToastNotify;

namespace FromLocalsToLocals.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IToastNotification _toastNotification;

        public AccountController(UserManager<AppUser> userManager , SignInManager<AppUser> signInManager, AppDbContext context, ILogger<AccountController> logger, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _toastNotification = toastNotification;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Email = model.Email,
                    UserName = model.Username
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent:false);
                    return RedirectToAction("index", "home");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }

                ModelState.AddModelError(string.Empty,"Invalid Login Attempt");
               
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }
                    
                //Code for sending email

                _toastNotification.AddSuccessToastMessage("Email sent");
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return View();
        }

        public IActionResult Profile()
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.Single(x => x.Id == userId);

            var model = GetNewProfileVM(user);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(string submitBtn,ProfileVM model)
        {
            switch (submitBtn)
            {
                case "picName":
                    return await PicNameChange(model);
                case "password":
                    return await ChangePassword(model);
                default:
                    return View();
            }
        }
        private async Task<IActionResult> PicNameChange(ProfileVM model)
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.Single(x => x.Id == userId);
            var oldModel = GetNewProfileVM(user);
            var resultsList = new List<IdentityResult>();

            if (model.UserName != user.UserName)
            {
                if(model.UserName == "")
                {
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.UserName)).Value.RawValue = user.UserName;
                    ModelState.AddModelError("", $"Username cannot be empty");
                }
                else if(!_context.Users.Any(x => x.UserName == model.UserName))
                {
                    var resUser = await _userManager.SetUserNameAsync(user, model.UserName);
                    resultsList.Add(resUser);
                }
                else
                {
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.UserName)).Value.RawValue = user.UserName;
                    ModelState.AddModelError("", $"Username '{model.UserName}' is already taken." );
                }
            }
            if (model.Email != user.Email)
            {
                if (model.Email == "")
                {
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.Email)).Value.RawValue = user.Email;
                    ModelState.AddModelError("", $"Email cannot be empty");
                }
                else if (!_context.Users.Any(x => x.Email == model.Email))
                {
                    var resEmail = await _userManager.SetEmailAsync(user, model.Email);
                    resultsList.Add(resEmail);
                }
                else
                {
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.Email)).Value.RawValue = user.Email;
                    ModelState.AddModelError("", $"Email '{model.Email}' is already in use.");
                }
            }
            if (model.ImageFile != null)
            {
                if (model.ImageFile.Length > 0)
                {
                    using (var target = new MemoryStream())
                    {
                        model.ImageFile.CopyTo(target);
                        user.Image = target.ToArray();
                        model.Image = user.Image;
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
            }
            var errors = GetErrors(resultsList);
            if (!errors.IsNullOrEmpty())
            {
                errors.ForEach(e => ModelState.AddModelError("", e));
            }
            
            if(ModelState.ErrorCount == 0)
            _toastNotification.AddSuccessToastMessage("Changes saved successfully");
            
            return Profile();
        }
        private async Task<IActionResult> ChangePassword(ProfileVM model)
        {
            if(string.IsNullOrWhiteSpace(model.OldPassword) || string.IsNullOrWhiteSpace(model.NewPassword) || string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Please, fill all fields");
                return Profile();
            }
            if (model.NewPassword.Length < 6)
            {
                ModelState.AddModelError("", "Password must be at least 6 characters long");
                return Profile();
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return Profile();
            }

            var user = await _userManager.GetUserAsync(User);
            
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return Profile();
            }

            await _signInManager.RefreshSignInAsync(user);
            _toastNotification.AddSuccessToastMessage("Password changed successfully");

            return Profile();
        }

        #region Helpers

        private List<string> GetErrors(List<IdentityResult> results)
        {
            var tempAns = new List<string>();

            foreach(var r in results)
            {
                if (!r.Succeeded)
                {
                    r.Errors.ForEach(x => tempAns.Add(x.Description));
                }
            }
            return tempAns;
        }

        public static ProfileVM GetNewProfileVM(AppUser user)
        {
            return new ProfileVM(user.Email, user.UserName, user.Image);
        }
        #endregion

    }
}
