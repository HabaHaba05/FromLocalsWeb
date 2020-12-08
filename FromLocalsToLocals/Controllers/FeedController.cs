using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FromLocalsToLocals.Database;
using FromLocalsToLocals.Models;
using FromLocalsToLocals.Models.Services;
using FromLocalsToLocals.Models.ViewModels;
using FromLocalsToLocals.Utilities;
using FromLocalsToLocals.Utilities.Enums;
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
        private readonly IPostsService _postsService;
        private readonly IVendorService _vendorService;
        private readonly AppDbContext _context;


        public FeedController(AppDbContext context, UserManager<AppUser> userManager, IToastNotification toastNotification, IPostsService postsService, IVendorService vendorService)
        {
            _context = context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            _postsService = postsService;
            _vendorService = vendorService;

        }

        [HttpGet]
        public IActionResult NewsFeed(FeedVM model)
        {
            return View(model);
        } 
        [HttpGet]
        public async Task<IActionResult> GetAllPostsAsync(int skip, int itemsCount)
        {
            return Json(await _postsService.GetAllPostsAsync(skip, itemsCount));
        }
        [HttpGet]
        public async Task<IActionResult> GetFollowingPostsAsync(string userId,int skip, int itemsCount)
        {
            return Json(await _postsService.GetFollowingPostsAsync(userId,skip, itemsCount));
        }
        [HttpGet]
        public async Task<IActionResult> GetVendorPostsAsync(int vendorId, int skip, int itemsCount)
        {
            return Json(await _postsService.GetVendorPostsAsync(vendorId,skip, itemsCount));
        }

        [Authorize]
        public IActionResult SwitchTabs(FeedVM model, string tabName)
        {
            model.ActiveTab = tabName.ParseEnum<FeedTabs>();
            return RedirectToAction(nameof(NewsFeed), model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost(CreatePostVM model)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Cannot create post with an empty message");
                return Redirect(model.PostBackUrl);
            }

            var userId = _userManager.GetUserId(User);
            var selectedVendor = await _vendorService.GetVendorAsync(userId, model.VendorTitle);

            if (selectedVendor != null)
            {
                try
                {
                    var post = new Post(model.Text, selectedVendor, model.Image.ConvertToBytes());
                    await _vendorService.AddPostAsync(selectedVendor,post);

                }
                catch (Exception)
                {
                    _toastNotification.AddErrorToastMessage("Something unexpected happened. Cannot create a post.");
                    return Redirect(model.PostBackUrl);
                }
            }

            model.Text = "";

            return Redirect(model.PostBackUrl);
        }
    }
}
