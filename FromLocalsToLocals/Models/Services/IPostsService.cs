using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FromLocalsToLocals.Models.Services
{
    public interface IPostsService
    {
        Task<IActionResult> GetAllPostsAsync(int skip, int take);
        Task<IActionResult> GetVendorPostsAsync(int vendorId,int skip, int take);
        Task<IActionResult> GetFollowingPostsAsync(string userId,int skip, int take);
    }
}
