using FromLocalsToLocals.Utilities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FromLocalsToLocals.Models.ViewModels
{
    public class FeedVM
    {
        public AppUser ActiveUser { get; set; }
        public FeedTabs ActiveTab { get; set; }

        public IEnumerable<Post> Posts { get; set; } 

        public bool DisplayInDetails { get; set; }
        public Vendor Vendor { get; set; }
    }
}
