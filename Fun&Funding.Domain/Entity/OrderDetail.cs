using System;
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
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
