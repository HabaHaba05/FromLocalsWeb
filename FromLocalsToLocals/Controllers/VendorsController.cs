﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FromLocalsToLocals.Database;
using FromLocalsToLocals.Models;
using SuppLocals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using FromLocalsToLocals.Utilities;
using FromLocalsToLocals.Models.Services;
using NToastNotify;
using FromLocalsToLocals.Models.ViewModels;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;

namespace FromLocalsToLocals.Controllers
{
    [Authorize]
    public class VendorsController : Controller
    {
        private readonly Lazy<UserManager<AppUser>> _userManager;
        private readonly IVendorServiceEF _vendorService;
        private readonly IToastNotification _toastNotification;
        private readonly AppDbContext _context;
        private readonly IStringLocalizer<VendorsController> _localizer;
        private readonly IVendorServiceADO _vendorServiceADO;

        public VendorsController(AppDbContext context, UserManager<AppUser> userManager, IVendorServiceEF vendorService, IToastNotification toastNotification, IStringLocalizer<VendorsController> localizer, IVendorServiceADO dataAdapterService)
        {
            _userManager = new Lazy<UserManager<AppUser>>(() => userManager);
            _vendorService = vendorService;
            _toastNotification = toastNotification;
            _context = context;
            _localizer = localizer;
            _vendorServiceADO = dataAdapterService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AllVendors([FromQuery(Name = "ordertype")] string? orderType,[FromQuery(Name = "vendortype")]string? vendorType, [FromQuery(Name = "searchString")] string? searchString, [FromQuery(Name = "page")] int? page, [FromQuery(Name = "itemCount")] int? itemCount)
        {
            List<VendorType> typesOfVendors = Enum.GetValues(typeof(VendorType)).Cast<VendorType>().ToList();
            List<OrderType> typesOfOrdering = Enum.GetValues(typeof(OrderType)).Cast<OrderType>().ToList();
            List<Vendor> newVendors = await _vendorService.GetNewVendorsAsync(count: 4);

            var vendors = await _vendorService.GetVendorsAsync(searchString, vendorType);
            vendors.ForEach(a => a.UpdateReviewsCount(_context));
            _vendorService.Sort(vendors, orderType ?? "");


            var vendorTypeVM = new VendorTypeViewModel
            {
                Types = new SelectList(typesOfVendors),
                OrderTypes = new SelectList(typesOfOrdering),
                Vendors = PaginatedList<Vendor>.Create(vendors, page ?? 1, itemCount ?? 8),
                NewVendors = newVendors
            };

            return View(vendorTypeVM);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> MyVendors()
        {
            return View(await _vendorService.GetVendorsAsync( userId : _userManager.Value.GetUserId(User)));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _vendorService.GetVendorAsync(id ?? default);

            if (vendor == null)
            {
                return NotFound();
            }

            await _vendorService.UpdatePopularityAsync(vendor);

            try 
            {
                vendor.FollowerCount = _context.Followers.Where(y => y.VendorID == vendor.ID).Count();
            }
            catch(Exception e)
            {
                await e.ExceptionSender();
            }

            var m = 0;
            return View(vendor);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEditVendorVM model)
        {
            try
            {
                if (ModelState.GetFieldValidationState("Title") == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid &&
                    ModelState.GetFieldValidationState("Address") == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid)
                {
                    if (model.Image != null && !model.Image.ValidImage())
                    {
                        ModelState.AddModelError("", _localizer["Invalid image type"]);
                        _toastNotification.AddErrorToastMessage(_localizer["Invalid image type"]);
                        return View(model);
                    }

                    var latLng = await MapMethods.ConvertAddressToLocationAsync(model.Address);

                    if (latLng == null)
                    {
                        ModelState.AddModelError("", _localizer["Sorry, we can't recognize this address"]);
                        _toastNotification.AddErrorToastMessage(_localizer["Sorry, we can't recognize this address"]);
                        return View(model);
                    }

                    var user = await _userManager.Value.GetUserAsync(User);
                    user.VendorsCount++;
                    await _userManager.Value.UpdateAsync(user);

                    var vendor = new Vendor();
                    vendor.UserID = user.Id;
                    vendor.Latitude = latLng.Item1;
                    vendor.Longitude = latLng.Item2;

                    model.SetValuesToVendor(vendor);
                    await _vendorService.CreateAsync(vendor);

                    var serviceOperatingHours = model.VendorHours;

                    foreach (var elem in serviceOperatingHours)
                    {
                        if (elem.IsWorking)
                        {
                            if (elem.CloseTime < elem.OpenTime)
                            {
                                ModelState.AddModelError("", "Invalid work hours");
                                _toastNotification.AddErrorToastMessage("Choose appropriate working hours");
                                await _vendorService.DeleteAsync(vendor);
                                return View(model);
                            }

                            else
                            {
                                var workHours = new WorkHours(vendor.ID, elem.IsWorking, elem.Day, elem.OpenTime, elem.CloseTime);
                                await _vendorService.AddWorkHoursAsync(workHours);
                            }
                        }
                        else
                        {
                            var timeSpan = new TimeSpan(0);
                            var workHours = new WorkHours(vendor.ID, elem.IsWorking, elem.Day, timeSpan, timeSpan);
                            await _vendorServiceADO.InsertWorkHoursAsync(workHours);
                        }
                    }

                    _toastNotification.AddSuccessToastMessage(_localizer["Service Created"]);
                    return RedirectToAction("MyVendors");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _toastNotification.AddErrorToastMessage(ex.Message);
            }
            
            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _vendorService.GetVendorAsync(id ?? default);

            if (vendor == null)
            {
                return NotFound();
            }

            if (!ValidUser(vendor.UserID))
            {
                return NotFound();
            }

            var workHours = _context.VendorWorkHours.Where(x => x.VendorID == vendor.ID).OrderBy(y => y.Day).ToList();
            
            return View(new CreateEditVendorVM(vendor, workHours));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateEditVendorVM model)
        {
            if (id != model.ID)
            {
                return NotFound();
            }

            var vendor = await _vendorService.GetVendorAsync(id);

            if (!ValidUser(vendor.UserID))
            {
                return NotFound();
            }

            if (ModelState.GetFieldValidationState("Title") == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid &&
                ModelState.GetFieldValidationState("Address") == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid)
            {
                if (model.Image != null && !model.Image.ValidImage())
                {
                    ModelState.AddModelError("", _localizer["Invalid image type"]);
                    _toastNotification.AddErrorToastMessage(_localizer["Invalid image type"]);
                    return View(model);
                }

                var latLng = await MapMethods.ConvertAddressToLocationAsync(model.Address);

                if (latLng == null)
                {
                    _toastNotification.AddErrorToastMessage(_localizer["Sorry, we can't recognize this address"]);
                    return View(model);
                }

                try
                {
                    vendor.Latitude = latLng.Item1;
                    vendor.Longitude = latLng.Item2;

                    model.SetValuesToVendor(vendor);

                    var serviceOperatingHours = model.VendorHours;
                    foreach (var elem in serviceOperatingHours)
                    {
                        if (elem.IsWorking)
                        {
                            if (elem.CloseTime < elem.OpenTime)
                            {
                                ModelState.AddModelError("", "Invalid work hours");
                                _toastNotification.AddErrorToastMessage("Choose appropriate working hours");
                                return View(model);
                            }

                            else
                            {
                                var workHours = new WorkHours(vendor.ID, elem.IsWorking, elem.Day, elem.OpenTime, elem.CloseTime);
                                await _vendorService.ChangeWorkHoursAsync(workHours);
                            }
                        }
                        else
                        {
                            var timeSpan = new TimeSpan(0);
                            var workHours = new WorkHours(vendor.ID, elem.IsWorking, elem.Day, timeSpan, timeSpan);
                            await _vendorService.ChangeWorkHoursAsync(workHours);
                        }
                    }

                    await _vendorServiceADO.UpdateVendorAsync(vendor);
                    _toastNotification.AddSuccessToastMessage(_localizer["Service Updated"]);

                    return RedirectToAction(nameof(MyVendors));
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    _toastNotification.AddErrorToastMessage(ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _vendorService.GetVendorAsync(id ?? default);

            if (vendor == null)
            {
                return NotFound();
            }

            if (!ValidUser(vendor.UserID))
            {
                return NotFound();
            }

            try
            {
                try
                {
                    _context.Notifications.RemoveRange(_context.Notifications.Where(x => x.VendorId == vendor.ID));
                    await _vendorServiceADO.DeleteVendorAsync(vendor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await e.ExceptionSender();
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _toastNotification.AddErrorToastMessage(ex.Message);
            }
            return RedirectToAction(nameof(MyVendors));
        }
        
        #region Helpers

        private bool ValidUser(string id)
        {
            return id == _userManager.Value.GetUserId(User);
        }
        #endregion
    }
}
