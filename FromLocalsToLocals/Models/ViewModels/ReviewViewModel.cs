using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FromLocalsToLocals.Models
{
    public class ReviewViewModel
    {
        public Vendor Vendor { get; set; }
        public IEnumerable<Tuple<Review,byte[]>> Reviews { get; set; }
    }
}

