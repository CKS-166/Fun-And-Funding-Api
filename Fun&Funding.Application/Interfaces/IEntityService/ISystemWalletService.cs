using Fun_Funding.Application.ViewModel;
using Fun_Funding.Domain.Entity;

namespace Fun_Funding.Application.Interfaces.IEntityService
{
    public interface ISystemWalletService
    {
        Task<ResultDTO<decimal>> GetPlatformRevenue();
        Task<ResultDTO<SystemWallet>> CreateWallet();
        Task<ResultDTO<SystemWallet>> GetSystemWallet();
        Task<ResultDTO<object>> GetDashboardMetrics();
        Task<ResultDTO<object>> GetDashboardUsers();
        Task<ResultDTO<object>> GetDashboardFundingProjects();
        Task<ResultDTO<object>> GetDashboardMarketplaceProjects();
        Task<ResultDTO<object>> GetDashboardMilestones();
    }
}
