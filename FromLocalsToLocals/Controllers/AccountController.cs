using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FromLocalsToLocals.Database;
using FromLocalsToLocals.Models;
using FromLocalsToLocals.ViewModels;
using Geocoding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FromLocalsToLocals.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<AppUser> userManager , SignInManager<AppUser> signInManager, AppDbContext context )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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
        [Authorize]
        public IActionResult Profile()
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.Single(x => x.Id == userId);

            var model = GetNewProfileVM(user);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(ProfileVM model)
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.Single(x => x.Id == userId);
            var oldModel = GetNewProfileVM(user);
            var resultsList = new List<IdentityResult>();

            if (model.UserName != user.UserName)
            {
                if(!_context.Users.Any(x => x.UserName == model.UserName))
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
                if (!_context.Users.Any(x => x.Email == model.Email))
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
