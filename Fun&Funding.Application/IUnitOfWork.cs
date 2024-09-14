using Fun_Funding.Application.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application
{
    public interface IUnitOfWork
    {
        // Repository interfaces
        IBankAccountRepository BankAccountRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IOrderRepository OrderRepository { get; }
        IPackageBackerRepository PackageBackerRepository { get; }
        IPackageRepository PackageRepository { get; }
        IProjectRepository ProjectRepository { get; }
        IRewardItemRepository RewardItemRepository { get; }
        ISourceFileRepository SourceFileRepository { get; }
        ISystemWalletRepository SystemWalletRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IUserRepository UserRepository { get; }
        IWalletRepository WalletRepository { get; }
        IWithdrawRequestRepository WithdrawRequestRepository { get; }

        // Methods for committing and rolling back
        void Commit();
        Task CommitAsync();
        void Rollback();
        Task RollbackAsync();
    }
}
