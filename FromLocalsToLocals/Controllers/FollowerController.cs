using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FromLocalsToLocals.Database;
using FromLocalsToLocals.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FromLocalsToLocals.Controllers
{
    [Authorize]
    public class FollowerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public FollowerController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Follow(int? id)
        {

            var user = await _userManager.Users.FirstOrDefaultAsync(x=> x.Id==_userManager.GetUserId(User));

            if (id == null || user == null)
            {
                return Json(new { success = false });
            }

            var vendor = await _context.Vendors.FirstOrDefaultAsync(x => x.ID == id);
            var follower = new Follower(user, vendor);
            try
            {
                user.Following.Add(follower);
                await _userManager.UpdateAsync(user);
            }
            catch(Exception ex)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Unfollow(int? id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x=> x.Id == _userManager.GetUserId(User));

            if (id == null || user == null)
            {
                return Json(new { success = false });
            }

            var followingVendor = user.Following.FirstOrDefault(x => x.VendorID == id);

            try
            {
                user.Following.Remove(followingVendor);
                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }
    }
}