using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fun_Funding.Domain.Enum;

namespace Fun_Funding.Domain.Entity
{
    public class ProjectCoupon : BaseEntity
    {
        public string CouponKey { get; set; }
        public string CouponName { get; set; }
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CommissionRate { get; set; }
        public ProjectCouponStatus Status { get; set; }
        public Guid MarketplaceProjectId { get; set; }
        public MarketplaceProject MarketplaceProject { get; set; }
    }
}
