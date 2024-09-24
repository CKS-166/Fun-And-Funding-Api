using Fun_Funding.Application.IService;
using Fun_Funding.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fun_Funding.Domain.Entity;

namespace Fun_Funding.Application.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateTransactionAsync(
            decimal totalAmount,
            string description,
            TransactionTypes transactionType,
            Guid packageId,
            Guid walletId,
            Guid? systemWalletId = null,
            Guid? commissionFeeId = null,
            Guid? orderId = null
        )
        {
            var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(walletId) ?? throw new Exception("Wallet not found");
            var transaction = new Transaction
            {
                Description = description,
                TotalAmount = totalAmount,
                TransactionType = transactionType,
                PackageId = packageId,
                WalletId = walletId,
                Wallet = wallet,
                SystemWalletId = systemWalletId,
                CommissionFeeId = commissionFeeId,
                OrderId = orderId ?? Guid.Empty
            };

            await _unitOfWork.TransactionRepository.AddAsync(transaction);
            await _unitOfWork.CommitAsync();

        }
    }
}
