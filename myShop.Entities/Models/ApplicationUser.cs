﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.Entities.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; } 

    }
}
