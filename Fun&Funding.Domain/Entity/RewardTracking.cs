using Fun_Funding.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Domain.Entity
{
    public class RewardTracking : BaseEntity
    {
        public DateTime ShipDate { get; set; }
        public TrackingStatus TrackingStatus { get; set; }
        public string Address { get; set; }

        public Guid BackerId { get; set; }
        public Guid PackageId { get; set; }
    }
}
