﻿using FromLocalsToLocals.Utilities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FromLocalsToLocals.Models.ViewModels
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
