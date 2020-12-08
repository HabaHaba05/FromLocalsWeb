using FromLocalsToLocals.Database;
using FromLocalsToLocals.Utilities;
using Geocoding;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using SuppLocals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FromLocalsToLocals.Models.Services
{
    public class VendorService : IVendorService
    {
        private readonly AppDbContext _context;


        public VendorService(AppDbContext context)
        {
            _context = context;
        }


        public async Task AddPostAsync(Vendor vendor,Post post)
        {
            vendor.Posts.Add(post);
            _context.Update(vendor);
            await _context.SaveChangesAsync();
        }

        public void Sort(List<Vendor> vendors, string order)
        {
            switch (order)
            {
                case "MostLiked":
                    vendors.Sort(delegate (Vendor t1, Vendor t2) { return (-1) * t1.CompareTo(t2); });
                    break;
                case "LeastLiked":
                    vendors.Sort(delegate (Vendor t1, Vendor t2) { return t1.CompareTo(t2); });
                    break;
            };
        }

        public async Task CreateAsync(Vendor vendor)
        {
            try
            {
                _context.Vendors.Add(vendor);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException e)
            {
                await e.ExceptionSender();
            }

        }

        public async Task DeleteAsync(Vendor vendor)
        {
            try
            {
                _context.Notifications.RemoveRange(_context.Notifications.Where(x => x.VendorId == vendor.ID));
                _context.Vendors.Remove(vendor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await e.ExceptionSender();
            }
        }
        public async Task<Vendor> GetVendorAsync(int id)
        {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(m => m.ID == id);

            vendor.VendorHours = await _context.VendorWorkHours.Where(x => x.VendorID == id).ToListAsync();
            return vendor;
        }

        public async Task<List<Vendor>> GetVendorsAsync(string searchString="", string vendorType="")
        {
            var vendors = from v in _context.Vendors
                                          select v;
            return await FilterVendorsListAsync(vendors, searchString, vendorType);
        }

        public async Task<List<Vendor>> GetVendorsAsync(string userId, string searchString="",string vendorType="")
        {
            var vendors = from v in _context.Vendors
                          where v.UserID == userId
                                          select v;
            return await FilterVendorsListAsync(vendors,searchString,vendorType);
        }

        public async Task<List<Vendor>> GetNewVendorsAsync(int count)
        {
            var list = _context.Vendors.OrderByDescending(v => v.DateCreated).Take(count);
            return await list.ToListAsync();
        }

        public async Task<List<Vendor>> GetPopularVendorsAsync(int count)
        {
            var list = _context.Vendors.Where(x => (DateTime.UtcNow - x.LastClickDate).Days < 7).OrderByDescending(x => x.Popularity).Take(count);
            return await list.ToListAsync(); 
        }

        public async Task AddWorkHoursAsync(WorkHours workHours)
        {
            try
            {
                _context.VendorWorkHours.Add(workHours);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await e.ExceptionSender();
            }
        }

        public async Task ChangeWorkHoursAsync(WorkHours workHours)
        {
            try
            {
                var row = _context.VendorWorkHours.FirstOrDefault(x => x.VendorID == workHours.VendorID && x.Day == workHours.Day);
                row.VendorID = workHours.VendorID;
                row.Day = workHours.Day;
                row.OpenTime = workHours.OpenTime;
                row.CloseTime = workHours.CloseTime;
                _context.VendorWorkHours.Update(row);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await e.ExceptionSender();
            }
        }

        public async Task UpdateAsync(Vendor vendor)
        {
            try
            {
                _context.Vendors.Update(vendor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await e.ExceptionSender();
                throw new DbUpdateException("Unable to update service in database");
            }
        }

        public bool Exists(int id)
        {
            return _context.Vendors.Any(e => e.ID == id);
        }

        private async Task<List<Vendor>> FilterVendorsListAsync(IQueryable<Vendor> vendors, string searchString = "", string vendorType = "")
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                vendors = vendors.Where(s => s.Title.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(vendorType))
            {
                vendors = vendors.Where(x => x.VendorTypeDb == vendorType);
            }
            return await vendors.ToListAsync();
        }

        public async Task<Vendor> GetVendorAsync(string userId, string title)
        {
            return await _context.Vendors.FirstOrDefaultAsync(x => x.Title == title && x.UserID == userId);
        }
    }

}
