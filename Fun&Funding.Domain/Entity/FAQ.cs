using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Domain.Entity
{
    public class FAQ : BaseEntity
    {
        public string Content { get; set; }
        public DateTime UpdateDate { get; set; }
        public FundingProject? FundingProject { get; set; }
        public Guid FundingProjectId { get; set; }
    }
}
