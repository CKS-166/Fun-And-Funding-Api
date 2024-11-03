using Fun_Funding.Application.Interfaces.IEntityService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.MarketplaceFileDTO;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class MarketplaceFileService : IMarketplaceFileService
    {
        public Task<ResultDTO<MarketplaceFileInfoResponse>> UploadUpdateGameFile(MarketplaceGameFileRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
