using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
using Fun_Funding.Domain.Enum;

namespace Fun_Funding.Application.IService
{
    public interface IFundingProjectService
    {
        Task<ResultDTO<FundingProjectResponse>> GetProjectById(Guid id);
        Task<ResultDTO<FundingProjectResponse>> CreateFundingProject(FundingProjectAddRequest req);
        Task<ResultDTO<string>> UpdateFundingProject(FundingProjectUpdateRequest req);
        Task<ResultDTO<FundingProjectResponse>> UpdateFundingProjectStatus(Guid id, ProjectStatus status);
    }
}
