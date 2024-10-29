using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.MarketplaceProjectDTO;
using Fun_Funding.Domain.Entity;

namespace Fun_Funding.Application.IService
{
    public interface IMarketplaceService
    {
        Task<ResultDTO<PaginatedResponse<MarketplaceProject>>> GetAllMarketplaceProject(ListRequest request);
        Task<ResultDTO<MarketplaceProjectInfoResponse>> CreateMarketplaceProject(MarketplaceProjectAddRequest request);
    }
}
