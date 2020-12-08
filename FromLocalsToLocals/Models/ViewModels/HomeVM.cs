using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FromLocalsToLocals.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Vendor> AllVendors { get; set; }
        public IEnumerable<Vendor> PopularVendors { get; set; }
    }
}
