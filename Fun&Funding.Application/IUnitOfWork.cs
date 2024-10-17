﻿using Fun_Funding.Application.IRepository;

namespace Fun_Funding.Application
{
    public interface IUnitOfWork
    {
        // Repository interfaces
        ILikeRepository LikeRepository { get; }
        ICommentRepository CommentRepository { get; }
        IFollowRepository FollowRepository { get; }
        IReportRepository ReportRepository { get; }
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
        IUserFileRepository UserFileRepository { get; }
        IWalletRepository WalletRepository { get; }
        IWithdrawRequestRepository WithdrawRequestRepository { get; }
        ICommissionFeeRepository CommissionFeeRepository { get; }
        IProjectCouponRepository ProjectCouponRepository { get; }
        IMilestoneRepository MilestoneRepository { get; }
        IMarketplaceRepository MarketplaceRepository { get; }
        IProjectMilestoneBackerRepository ProjectMilestoneBackerRepository { get; }
        IProjectMilestoneRepository ProjectMilestoneRepository { get; }
        IRequirementRepository RequirementRepository { get; }
        IProjectMilestoneRequirementRepository ProjectMilestoneRequirementRepository { get; }
        IProjectRequirementFileRepository ProjectRequirementFileRepository { get; }

        ICreatorContractRepository CreatorContractRepository { get; }
        
        // Methods for committing and rolling back
        void Commit();
        Task CommitAsync();
        void Rollback();
        Task RollbackAsync();
    }
}
