using Fun_Funding.Application.ViewModel.ProjectMilestoneDTO;
using Fun_Funding.Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fun_Funding.Application.ViewModel.ProjectMilestoneRequirementDTO;

namespace Fun_Funding.Application.IService
{
    public interface IProjectMilestoneRequirementService
    {
        Task<ResultDTO<string>> CreateMilestoneRequirements(List<ProjectMilestoneRequirementRequest> request);
        Task<ResultDTO<string>> UpdateMilestoneRequirements(List<ProjectMilestoneRequirementUpdateRequest> request);
    }
}
