using FromLocalsToLocals.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace FromLocalsToLocals.Models.Services
{
    public class PostsService : IPostsService
    {
        private readonly AppDbContext _context;

        public PostsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetAllPostsAsync(int skip, int take)
        {
            try
            {
                return new JsonResult(await _context.Posts.OrderByDescending(x => x).Skip(skip).Take(take).ToListAsync());
            }
            catch
            {
                throw new Exception("Something unexpected happened while trying to load posts");
            }

        }

        public async Task<IActionResult> GetFollowingPostsAsync(string userId, int skip, int take)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                var listOfPosts = new List<Post>();
                foreach(var follower in user.Following)
                {
                    foreach(var p in follower.Vendor.Posts)
                    {
                        listOfPosts.Add(p);
                    }
                }

                listOfPosts = listOfPosts.OrderByDescending(x => x.Date).Skip(skip).Take(take).ToList();

                return new JsonResult(listOfPosts);

            }
            catch
            {
                throw new Exception("Something unexpected happened while trying to load posts");
            }
        }

        public async Task<IActionResult> GetVendorPostsAsync(int vendorId, int skip, int take)
        {
            try
            {
                return new  JsonResult(await _context.Posts.Where(x => x.VendorID == vendorId).OrderByDescending(x => x).Skip(skip).Take(take).ToListAsync());
            }
            catch
            {
                throw new Exception("Something unexpected happened while trying to load posts");
            }
        }
    }
}
