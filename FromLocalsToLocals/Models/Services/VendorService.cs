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
using static FromLocalsToLocals.Utilities.TimeCalculator;

namespace FromLocalsToLocals.Models.Services
{
    public class VendorService : IVendorService
    {
        private readonly AppDbContext _context;


        public VendorService(AppDbContext context)
        {
            _context = context;
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
            return await _context.Vendors.FirstOrDefaultAsync(m => m.ID == id);
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

    }

}
