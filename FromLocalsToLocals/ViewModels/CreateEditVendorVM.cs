using FromLocalsToLocals.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FromLocalsToLocals.ViewModels
{
    public class CreateEditVendorVM
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }

        public string About { get; set; }

        [Required]
        public string Address { get; set; }

        [Required] public VendorType VendorType { get; set; }

        public IFormFile Image { get; set; }

    }
}
