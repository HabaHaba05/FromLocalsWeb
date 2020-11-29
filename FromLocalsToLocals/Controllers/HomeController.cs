﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FromLocalsToLocals.Models;
using FromLocalsToLocals.Models.Services;
using Microsoft.AspNetCore.Authorization;
using FromLocalsToLocals.ViewModels;
using SuppLocals;
using SendGrid;
using SendGrid.Helpers.Mail;
using NToastNotify;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using FromLocalsToLocals.Models.ViewModels;
using FromLocalsToLocals.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FromLocalsToLocals.Database;

namespace FromLocalsToLocals.Controllers
{
    public class HomeController : Controller
    {
        private readonly IToastNotification _toastNotification;

        private readonly IVendorService _vendorService;

        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;


        public HomeController(AppDbContext context, IStringLocalizer<HomeController> localizer, IVendorService vendorService, IToastNotification toastNotification, UserManager<AppUser> userManager )
        {
            _localizer = localizer;
            _vendorService = vendorService;
            _toastNotification = toastNotification;
            _userManager = userManager;
            _context = context;

        }

        public async Task<IActionResult> Index(string searchString)
        {
            return View(await _vendorService.GetVendorsAsync(searchString, ""));
        }

        public IActionResult ReportBug()
        {
            return View();
        }


        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Reviews()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ReportBug(BugReportVM model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(model.TextBug)) {
                    await Execute();
                    _toastNotification.AddSuccessToastMessage("Report message send succesfully!");
                    return View("Index");
                }

                else
                {
                    _toastNotification.AddErrorToastMessage("Report message can not be empty! ");
                    return View();
                }
            }

            async Task Execute()
            {
                var email = Config.email;
                var key = Config.Send_Grid_Key;
                var client = new SendGridClient(key);

                var from = new EmailAddress("fromlocalstolocals@gmail.com", "Bug reporter");
                var subject = "Found bug";
                var to = new EmailAddress(email , "Dear User");
                var plainTextContent = "";

                var htmlContent = "<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head><body>" + model.TextBug;
      
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }

  

            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) }
                );

            return LocalRedirect(returnUrl);
        }
    
        [HttpGet]
        public async Task<IActionResult> NewsFeed(PostVM model)
        {

            if (model.User == null)
            {
                model.User = await _userManager.Users.Include(x => x.Vendors)
                                .Include(x=>x.Folllowing).ThenInclude(x=>x.Vendor).ThenInclude(x=>x.Posts)
                                .FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));                
            }
            if (model.ActiveTab == null)
            {
                model.ActiveTab = Tab.AllFeed;
            }

            return View(model);
        }

        [Authorize]
        public IActionResult SwitchTabs(PostVM model,string tabName)
        {
            model.ActiveTab = tabName.ParseEnum<Tab>();
            return RedirectToAction(nameof(HomeController.NewsFeed), model);
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