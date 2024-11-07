﻿using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.MarketplaceProjectDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;

namespace Fun_Funding.Application.IService
{
    public interface IMarketplaceService
    {
        Task<ResultDTO<PaginatedResponse<MarketplaceProject>>> GetAllMarketplaceProject(ListRequest request);
        Task<ResultDTO<MarketplaceProjectInfoResponse>> CreateMarketplaceProject(MarketplaceProjectAddRequest request);
        Task<ResultDTO<MarketplaceProjectInfoResponse>> GetMarketplaceProjectById(Guid id);
        Task DeleteMarketplaceProject(Guid id);
        Task<ResultDTO<MarketplaceProjectInfoResponse>>
            UpdateMarketplaceProject(Guid id, MarketplaceProjectUpdateRequest request, bool? isDeleted = null);
        Task<ResultDTO<MarketplaceProjectInfoResponse>>
            UpdateMarketplaceProjectStatus(Guid id, ProjectStatus status);
        Task<ResultDTO<List<MarketplaceProjectInfoResponse>>> GetTop3MostFundedOngoingMarketplaceProject();
        Task<ResultDTO<decimal>> CountPlatformProjects();
    }
}
