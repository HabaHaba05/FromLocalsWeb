using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FromLocalsToLocals.Database;
using FromLocalsToLocals.Models;
using FromLocalsToLocals.Models.ViewModels;
using FromLocalsToLocals.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace FromLocalsToLocals.Controllers
{
    public class FeedController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IToastNotification _toastNotification;
        private readonly AppDbContext _context;


        public FeedController(UserManager<AppUser> userManager, IToastNotification toastNotification, AppDbContext context)
        {
            _userManager = userManager;
            _toastNotification = toastNotification;
            _context = context;

        }

        [HttpGet]
        public async Task<IActionResult> NewsFeed(PostVM model)
        {

            if (model.User == null)
            {
                model.User = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));
            }
            if (model.ActiveTab == null)
            {
                model.ActiveTab = Tab.AllFeed;
            }

            return View(model);
        }

        [Authorize]
        public IActionResult SwitchTabs(PostVM model, string tabName)
        {
            model.ActiveTab = tabName.ParseEnum<Tab>();
            return RedirectToAction(nameof(NewsFeed), model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost(PostVM model)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Cannot create post with empty message");
                return Redirect(model.PostBackUrl);
            }

            var userId = _userManager.GetUserId(User);
            model.SelectedVendor = await _context.Vendors.FirstOrDefaultAsync(x => x.Title == model.SelectedVendorTitle && x.UserID == userId);

            if (model.SelectedVendor != null)
            {
                try
                {
                    _context.Posts.Add(new Post(model));
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    _toastNotification.AddErrorToastMessage("Something unexpected happened. Cannot create a post.");
                    return Redirect(model.PostBackUrl);
                }
            }

            model.PostText = "";

            return Redirect(model.PostBackUrl);
        }
    }
}
