using Fun_Funding.Application.ViewModel.MilestoneDTO;
using Fun_Funding.Application.ViewModel.ProjectMilestoneRequirementDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.ViewModel.ProjectMilestoneDTO
{
    public class ProjectMilestoneResponse
    {
        public DateTime CreatedDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProjectMilestoneStatus Status { get; set; }

        public Guid MilestoneId { get; set; }
        public MilestoneResponse Milestone { get; set; }
        public Guid FundingProjectId { get; set; }
        public List<ProjectMilestoneRequirementResponse> ProjectMilestoneRequirements { get; set; }
    }
}
