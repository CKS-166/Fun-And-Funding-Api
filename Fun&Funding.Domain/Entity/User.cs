﻿using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Domain.Entity
{
    public class User : IdentityUser<Guid>
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public SourceFile Avatar { get; set; }
        public string? Address { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public UserStatus UserStatus { get; set; }
        [InverseProperty("Backer")]
        public Wallet? Wallet { get; set; }
        public virtual ICollection<PackageBacker> PackageUsers { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }

    }
}
