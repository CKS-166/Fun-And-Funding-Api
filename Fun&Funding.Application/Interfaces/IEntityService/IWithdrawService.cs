using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.WithdrawDTO;
using Fun_Funding.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IWithdrawService
    {
        public Task<ResultDTO<List<WithdrawRequest>>> GetAllRequest();
        public Task<ResultDTO<WithdrawResponse>>CreateMarketplaceRequest(Guid MarketplaceId);
        public Task<ResultDTO<AdminResponse>> AdminProcessingRequest(Guid id);
        public Task<ResultDTO<WithdrawRequest>> AdminCancelRequest(Guid id);
        public Task<ResultDTO<WithdrawRequest>> AdminApproveRequest(Guid id);
        Task<ResultDTO<string>> WalletWithdrawRequest();
    }
}
