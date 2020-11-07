using Microsoft.AspNetCore.Identity;


namespace FromLocalsToLocals.Models
{
    public class AppUser : IdentityUser
    {
        public byte[] Image { get; set; }
        public int VendorsCount { get; set; }
    }
}
