using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.Interfaces.IEntityService;
using Fun_Funding.Application.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class SystemWalletService : ISystemWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SystemWalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResultDTO<decimal>> GetPlatformRevenue()
        {
            try
            {
                var systemWallet = await _unitOfWork.SystemWalletRepository.GetQueryable().SingleOrDefaultAsync();
                if(systemWallet == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "No system wallet found.");
                }
                var balance = (await _unitOfWork.SystemWalletRepository.GetQueryable().SingleOrDefaultAsync())?.TotalAmount ?? 0;
                return ResultDTO<decimal>.Success(balance, "Platform balance");
            }
            catch (Exception ex)
            {
                if (ex is ExceptionError exceptionError)
                {
                    throw exceptionError;
                }
                throw new ExceptionError((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
