using Fun_Funding.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Domain.Entity
{
    public class Order : BaseEntity
    {
        public OrderStatus OrderStatus { get; set; }
       
        public Guid UserId { get; set; }
        public User User { get; set; }

        public virtual ICollection<RefundRequest> RefundRequests{ get; set; }


    }
}

