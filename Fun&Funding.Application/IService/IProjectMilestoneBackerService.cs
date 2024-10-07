using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ProjectMilestoneBackerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IProjectMilestoneBackerService
    {
        Task<ResultDTO<ProjectMilestoneBackerResponse>> CreateNewProjectMilestoneBackerReview(ProjectMilestoneBackerRequest request);
    }
}
