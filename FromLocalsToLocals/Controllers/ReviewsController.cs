﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FromLocalsToLocals.Database;
using FromLocalsToLocals.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using FromLocalsToLocals.Utilities;
using System.Collections.Generic;
using FromLocalsToLocals.Models.Services;

namespace FromLocalsToLocals.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IReviewsService _reviewsService;
        private readonly INotificationService _notificationService;



        public ReviewsController(AppDbContext context, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IHubContext<NotificationHub> hubContext, IReviewsService reviewsService, INotificationService notificationService)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _hubContext = hubContext;
            _reviewsService = reviewsService;
            _notificationService = notificationService;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Reviews()
        {
            var id = GetVendorID();
            var vendor = await _context.Vendors.FindAsync(id);
            vendor.UpdateReviewsCount(_context);

            var reviews = await _reviewsService.GetReviewsAsync(id);
            var users = await _context.Users.ToListAsync();

            var anonym = new AppUser{UserName = "Anonimas"};
            
            users.Add(anonym);

            var model = new ReviewViewModel
            {
                Vendor = vendor,
                Reviews = reviews.Join(users,
                    rev => rev.SenderUsername,
                    kit => kit.UserName,
                    (rev, kit) => Tuple.Create(rev, kit.Image))
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reviews(ReviewViewModel model)
        {
            var id = GetVendorID();
            var vendor = await _context.Vendors.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (vendor == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["vendorReply"]))
            {
                var index = int.Parse(Request.Form["postReview"]);

                await _reviewsService.AddReplyAsync(id, index, Request.Form["vendorReply"], vendor.Title); 
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["comment"]))
            {
                var commentId = int.Parse(Request.Form["listItemCount"]);
                var stars     = int.Parse(Request.Form["starRating"]);
                var userName = (user != null) ? user.UserName : "Anonimas";
                var review = new Review(id, commentId, userName, Request.Form["comment"], stars);

                await _reviewsService.CreateAsync(review);

                vendor.UpdateReviewsCount(_context);

                // Notify vendor owner that someone commented on his shop
                await NotifyUserWithNewReview(review, vendor.Title);
            }

            vendor.UpdateReviewsCount(_context);

            return await Reviews();
        }

        private async Task NotifyUserWithNewReview(Review review, string vendorTitle )
        {
            var id = GetVendorID();

            var notification = new Notification
            {
                OwnerId = _context.Vendors.FirstOrDefault(v => v.ID == GetVendorID()).UserID,
                VendorId = id,
                CreatedDate = DateTime.UtcNow,
                Review = review,
                NotiBody = $"{review.SenderUsername} gave {review.Stars} stars to '{vendorTitle}'.",
                Url = HttpContext.Request.Path.Value
            };

             await _notificationService.AddNotificationAsync(notification);
             await _hubContext.Clients.All.SendAsync("displayNotification", "");
        }

        private int GetVendorID()
        {
            string path = HttpContext.Request.Path.Value;
            return int.Parse(path.Remove(0, 26));
        }
    }
}
