using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FromLocalsToLocals.Database;
using FromLocalsToLocals.Models;
using SuppLocals;
using Microsoft.AspNetCore.Authorization;
using FromLocalsToLocals.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Policy;
using System;
using System.Diagnostics;
using System.IO;

namespace FromLocalsToLocals.Controllers
{
    [Authorize]
    public class VendorsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public VendorsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AllVendors()
        {
            return View(await _context.Vendors.ToListAsync());
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> MyVendors()
        {
            return View(await _context.Vendors.Where(x => x.UserID == _userManager.GetUserId(User)).ToListAsync());
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (vendor == null)
            {
                return NotFound();
            }

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
            if (ModelState.IsValid)
            {
                var latLng = await MapMethods.ConvertAddressToLocationAsync(model.Address);

                if (latLng != null)
                {
                    var vendor = new Vendor();
                    
                    vendor.UserID = _userManager.GetUserId(User);
                    vendor.Latitude = latLng.Item1;
                    vendor.Longitude = latLng.Item2;
                    vendor.Title = model.Title;
                    vendor.About = model.About;
                    vendor.Address = model.Address;

                    if (model.Image != null)
                    {
                        if (model.Image.Length > 0)
                        {
                            using (var target = new MemoryStream())
                            {
                                model.Image.CopyTo(target);
                                vendor.Image = target.ToArray();
                            }
                        }
                    }

                    _context.Vendors.Add(vendor);
                    _context.SaveChanges();

                    return RedirectToAction("MyVendors");
                }
                else
                {
                    //Inform the client-side that address is not recognizable
                }
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
            
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            if (!ValidUser(vendor.UserID))
            {
                return NotFound();
            }

            var model = new CreateEditVendorVM
            {
                ID = vendor.ID,
                Title = vendor.Title,
                About = vendor.About,
                Address = vendor.Address
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateEditVendorVM model)
        {
            if (id != model.ID)
            {
                return NotFound();
            }

            var vendor = _context.Vendors.Single(x => x.ID == id);

            if (!ValidUser(vendor.UserID))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var latLng = await MapMethods.ConvertAddressToLocationAsync(model.Address);

                    if (latLng == null)
                    {
                        //Somehow we should inform the client-side that probably address is not recognizable
                        return View(model);
                    }

                    if (model.Image != null)
                    {
                        if (model.Image.Length > 0)
                        {
                            using (var target = new MemoryStream())
                            {
                                model.Image.CopyTo(target);
                                vendor.Image = target.ToArray();
                            }
                        }
                    }

                    vendor.Title = model.Title;
                    vendor.About = model.About;
                    vendor.Address = model.Address;
                    vendor.Latitude = latLng.Item1;
                    vendor.Longitude = latLng.Item2;
                    vendor.VendorType = model.VendorType;
                    

                    _context.Update(vendor);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendorExists(model.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyVendors));
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

            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (vendor == null)
            {
                return NotFound();
            }

            if (!ValidUser(vendor.UserID))
            {
                return NotFound();
            }

            return View(vendor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);

            if (!ValidUser(vendor.UserID))
            {
                return NotFound();
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyVendors));
        }

        

        #region Helpers
        private bool VendorExists(int id)
        {
            return _context.Vendors.Any(e => e.ID == id);
        }

        private bool ValidUser(string id)
        {
            return id == _userManager.GetUserId(User);
        }

        #endregion

    }
}
