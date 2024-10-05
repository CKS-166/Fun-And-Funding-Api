using Fun_Funding.Application.IRepository;

namespace Fun_Funding.Application
{
    public interface IUnitOfWork
    {
        // Repository interfaces
        ILikeRepository likeRepository { get; }
        ICommentRepository commentRepository { get; }
        IBankAccountRepository BankAccountRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IOrderRepository OrderRepository { get; }
        IPackageBackerRepository PackageBackerRepository { get; }
        IPackageRepository PackageRepository { get; }
        IFundingProjectRepository FundingProjectRepository { get; }
        IRewardItemRepository RewardItemRepository { get; }
        ISourceFileRepository SourceFileRepository { get; }
        ISystemWalletRepository SystemWalletRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IUserRepository UserRepository { get; }
        IWalletRepository WalletRepository { get; }
        IWithdrawRequestRepository WithdrawRequestRepository { get; }
        ICommissionFeeRepository CommissionFeeRepository { get; }

        // Methods for committing and rolling back
        void Commit();
        Task CommitAsync();
        void Rollback();
        Task RollbackAsync();
    }
}
