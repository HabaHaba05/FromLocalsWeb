

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace FromLocalsToLocals.Models
{
    public class Follower
    {
        public Follower()
        {}

        public Follower(AppUser user, Vendor vendor)
        {
            User = user;
            Vendor = vendor;
        }

        public string UserID { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual AppUser User { get; set; }

        public int VendorID { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Vendor Vendor { get; set; }
    }
}
