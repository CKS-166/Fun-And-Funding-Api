using Fun_Funding.Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Interfaces.IEntityService
{
    public interface ISystemWalletService
    {
        Task<ResultDTO<decimal>> GetPlatformRevenue();
    }
}
