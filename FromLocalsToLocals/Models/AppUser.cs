using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FromLocalsToLocals.Models
{
    public class AppUser : IdentityUser
    {
        public byte[] Image { get; set; }
        public int VendorsCount { get; set; }

        public bool Subscribe { get; set; }

        public virtual ICollection<Vendor> Vendors { get; set; }
        public  ICollection<Follower> Folllowing { get; set; }

    }
}
