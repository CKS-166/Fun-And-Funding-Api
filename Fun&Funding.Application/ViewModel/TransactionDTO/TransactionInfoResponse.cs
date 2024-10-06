﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.ViewModel.TransactionDTO
{
    public class TransactionInfoResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Description { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid? PackageId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? CommissionFeeId { get; set; }
    }
}
