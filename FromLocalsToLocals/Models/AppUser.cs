using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace FromLocalsToLocals.Models
{
    public class AppUser : IdentityUser
    {
        public byte[] Image { get; set; }
        public int VendorsCount { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Vendor> Vendors { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Follower> Following { get; set; }

        public bool Subscribe { get; set; }

    }
}
