﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.ViewModel.ProjectCouponDTO
{
    public class AppliedCoupon
    {
        public Guid Id { get; set; }
        public string CouponKey { get; set; } = string.Empty;
        public decimal CommissionRate { get; set; }
    }
}
