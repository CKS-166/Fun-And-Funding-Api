﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Domain.Entity
{
    public class OrderDetail : BaseEntity
    {
        [Required]
        public Guid DigitalKeyID { get; set; }
        [Required]
        public DigitalKey DigitalKey { get; set; }

        [Required]
        public Guid OrderId { get; set; }
        [Required]
        public Order Order { get; set; }
    }
}
