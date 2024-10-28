using Fun_Funding.Application.Interfaces.IEntityService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class BankAccountService :IBankAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BankAccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResultDTO<BankAccount>> GetBankAccountByWalletId(Guid id)
        {
            try
            {
                var result = await _unitOfWork.BankAccountRepository.GetAsync(x => x.Wallet.Id == id);
                if (result is null)
                    return ResultDTO<BankAccount>.Fail("not found");
                return ResultDTO<BankAccount>.Success(result, "Successfully found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
