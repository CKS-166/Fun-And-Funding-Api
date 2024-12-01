using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.Interfaces.IEntityService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Domain.Entity;
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

        public async Task<ResultDTO<SystemWallet>> CreateWallet()
        {
            try
            {
                var wallet = new SystemWallet
                {
                    Id = new Guid(),        
                    CreatedDate = DateTime.Now,
                    TotalAmount = 0
                };
                _unitOfWork.SystemWalletRepository.Add(wallet);
                await _unitOfWork.CommitAsync();
                return ResultDTO<SystemWallet>.Success(wallet);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
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

        public async Task<ResultDTO<SystemWallet>> GetSystemWallet()
        {
            try
            {
                var systemWallet = await _unitOfWork.SystemWalletRepository.GetQueryable().SingleOrDefaultAsync();
                return ResultDTO<SystemWallet>.Success(systemWallet);
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
