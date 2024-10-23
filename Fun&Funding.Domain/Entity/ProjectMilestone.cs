using Fun_Funding.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Domain.Entity
{
    public class ProjectMilestone : BaseEntity
    {
        public DateTime EndDate { get; set; }
        public ProjectMilestoneStatus Status { get; set; }
        public Guid MilestoneId { get; set; }
        public Milestone Milestone { get; set; }
        public Guid FundingProjectId { get; set; }
        public FundingProject FundingProject { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<ProjectMilestoneBacker> ProjectMilestoneBackers { get; set; }
        public virtual ICollection<ProjectMilestoneRequirement> ProjectMilestoneRequirements { get; set; }
    }
}
