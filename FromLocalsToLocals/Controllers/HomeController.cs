﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SendGrid;
using SendGrid.Helpers.Mail;
using NToastNotify;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;
using FromLocalsToLocals.Contracts.Entities;
using FromLocalsToLocals.Utilities.Helpers;
using FromLocalsToLocals.Database;
using FromLocalsToLocals.Web.ViewModels;
using FromLocalsToLocals.Services.EF;

namespace FromLocalsToLocals.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IToastNotification _toastNotification;

        private readonly IVendorService _vendorService;

        private readonly IStringLocalizer<HomeController> _localizer;


        public HomeController(IStringLocalizer<HomeController> localizer, IVendorService vendorService,
            IToastNotification toastNotification)
        {
            _localizer = localizer;
            _vendorService = vendorService;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index(HomeVM homeVM)
        {
            homeVM.AllVendors = await _vendorService.GetVendorsAsync("", "");

            var popularVendors = await _vendorService.GetPopularVendorsAsync(4);
            homeVM.PopularVendors = popularVendors;

            return View(homeVM);
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

        public IActionResult Privacy()
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
                    _toastNotification.AddSuccessToastMessage(_localizer["Report message send succesfully!"]);
                    return View("Index");
                }

                else
                {
                    _toastNotification.AddErrorToastMessage(_localizer["Report message can not be empty!"]);
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
    }
}