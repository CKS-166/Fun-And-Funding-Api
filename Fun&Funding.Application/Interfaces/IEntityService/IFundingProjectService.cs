using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IFundingProjectService
    {
        Task<ResultDTO<FundingProjectResponse>> GetProjectById(Guid id);
        Task<ResultDTO<FundingProjectResponse>> CreateFundingProject(FundingProjectAddRequest req);
        Task<ResultDTO<FundingProjectResponse>> UpdateFundingProject(FundingProjectUpdateRequest req);
        Task<ResultDTO<PaginatedResponse<FundingProjectResponse>>> GetFundingProjects(ListRequest request, string? categoryName, ProjectStatus? status, decimal? fromTarget, decimal? toTarget);
        Task<ResultDTO<FundingProjectResponse>> UpdateFundingProjectStatus(Guid id, ProjectStatus status);
        Task<ResultDTO<bool>> CheckProjectOwner(Guid projectId);
        Task<ResultDTO<List<FundingProjectResponse>>> GetTop3MostFundedOngoingProject();
    }
}
