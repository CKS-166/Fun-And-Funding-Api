using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.WalletDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IWalletService
    {
        Task<ResultDTO<WalletInfoResponse>> GetWalletByUser(Guid userId);
        Task<ResultDTO<WalletInfoResponse>> AddMoneyToWallet(WalletRequest walletRequest);
    }
}
