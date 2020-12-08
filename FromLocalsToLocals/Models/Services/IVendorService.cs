

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FromLocalsToLocals.Models.Services
{
    public interface IVendorService
    {
        Task AddPostAsync(Vendor vendor,Post post);
        Task CreateAsync(Vendor vendor);

        Task<Vendor> GetVendorAsync(int id);
        Task<Vendor> GetVendorAsync(string userId, string title);
        Task<List<Vendor>> GetVendorsAsync(string searchString="", string vendorType = "");
        Task<List<Vendor>> GetVendorsAsync(string userId,string searchString="" , string vendorType="");
        Task<List<Vendor>> GetNewVendorsAsync(int count);
        Task<List<Vendor>> GetPopularVendorsAsync(int count);

        Task AddWorkHoursAsync(WorkHours workHours);
        Task ChangeWorkHoursAsync(WorkHours workHours);


        Task UpdateAsync(Vendor vendor);
        Task DeleteAsync(Vendor vendor);
        bool Exists(int id);
        void Sort(List<Vendor> vendors, string order);
    }
}
