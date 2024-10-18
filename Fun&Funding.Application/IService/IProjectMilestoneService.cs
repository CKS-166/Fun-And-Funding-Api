using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ProjectMilestoneDTO;
using Fun_Funding.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IProjectMilestoneService
    {
        Task<ResultDTO<ProjectMilestoneResponse>> CreateProjectMilestoneRequest(ProjectMilestoneRequest request);
        Task<ResultDTO<ProjectMilestoneResponse>> GetProjectMilestoneRequest(Guid id);
        Task<ResultDTO<List<ProjectMilestoneResponse>>> GetAllProjectMilestone();
        Task<ResultDTO<string>> UpdateProjectMilestoneStatus(ProjectMilestoneStatusUpdateRequest request);
        Task<ResultDTO<PaginatedResponse<ProjectMilestoneResponse>>> GetProjectMilestones(
             ListRequest request,
             ProjectMilestoneStatus? status,
             Guid? fundingProjectId);
    }
}

