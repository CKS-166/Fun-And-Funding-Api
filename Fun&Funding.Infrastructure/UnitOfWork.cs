using Fun_Funding.Application;
using Fun_Funding.Application.IRepository;
using Fun_Funding.Infrastructure.Database;
using Fun_Funding.Infrastructure.Repository;

namespace Fun_Funding.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyDbContext _dbContext;
        private readonly MongoDBContext _mongoDBContext;

        // Repositories
        private ILikeRepository _likeRepository;
        private ICommentRepository _commentRepository;
        private IBankAccountRepository _bankAccountRepository;
        private ICategoryRepository _categoryRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IOrderRepository _orderRepository;
        private IPackageBackerRepository _packageBackerRepository;
        private IPackageRepository _packageRepository;
        private IFundingProjectRepository _fundingProjectRepository;
        private IRewardItemRepository _rewardItemRepository;
        private ISourceFileRepository _sourceFileRepository;
        private ISystemWalletRepository _systemWalletRepository;
        private ITransactionRepository _transactionRepository;
        private IUserRepository _userRepository;
        private IWalletRepository _walletRepository;
        private IWithdrawRequestRepository _withdrawRequestRepository;
        private ICommissionFeeRepository _commissionFeeRepository;

        private IProjectCouponRepository _projectCouponRepository;
        private IMilestoneRepository _milestoneRepository;
        private IProjectMilestoneBackerRepository _projectMilestoneBackerRepository;
        private IProjectMilestoneRepository _projectMilestoneRepository;
        private IRequirementRepository _requirementRepository;
        private IProjectMilestoneRequirementRepository _projectMilestoneRequirementRepository;
        private IProjectRequirementFileRepository _projectRequirementFileRepository;

        public UnitOfWork(MyDbContext dbContext, MongoDBContext mongoDBContext)
        {
            _dbContext = dbContext;
            _mongoDBContext = mongoDBContext;
        }

        // Repository properties
        public IBankAccountRepository BankAccountRepository
        {
            get
            {
                return _bankAccountRepository = _bankAccountRepository ?? new BankAccountRepository(_dbContext);
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                return _categoryRepository = _categoryRepository ?? new CategoryRepository(_dbContext);
            }
        }

        public IOrderDetailRepository OrderDetailRepository
        {
            get
            {
                return _orderDetailRepository = _orderDetailRepository ?? new OrderDetailRepository(_dbContext);
            }
        }

        public IOrderRepository OrderRepository
        {
            get
            {
                return _orderRepository = _orderRepository ?? new OrderRepository(_dbContext);
            }
        }

        public IPackageBackerRepository PackageBackerRepository
        {
            get
            {
                return _packageBackerRepository = _packageBackerRepository ?? new PackageBackerRepository(_dbContext);
            }
        }

        public IPackageRepository PackageRepository
        {
            get
            {
                return _packageRepository = _packageRepository ?? new PackageRepository(_dbContext);
            }
        }

        public IFundingProjectRepository FundingProjectRepository
        {
            get
            {
                return _fundingProjectRepository = _fundingProjectRepository ?? new FundingProjectRepository(_dbContext);
            }
        }

        public IRewardItemRepository RewardItemRepository
        {
            get
            {
                return _rewardItemRepository = _rewardItemRepository ?? new RewardItemRepository(_dbContext);
            }
        }

        public ISourceFileRepository SourceFileRepository
        {
            get
            {
                return _sourceFileRepository = _sourceFileRepository ?? new SourceFileRepository(_dbContext);
            }
        }

        public ISystemWalletRepository SystemWalletRepository
        {
            get
            {
                return _systemWalletRepository = _systemWalletRepository ?? new SystemWalletRepository(_dbContext);
            }
        }

        public ITransactionRepository TransactionRepository
        {
            get
            {
                return _transactionRepository = _transactionRepository ?? new TransactionRepository(_dbContext);
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository = _userRepository ?? new UserRepository(_dbContext);
            }
        }

        public IWalletRepository WalletRepository
        {
            get
            {
                return _walletRepository = _walletRepository ?? new WalletRepository(_dbContext);
            }
        }

        public IWithdrawRequestRepository WithdrawRequestRepository
        {
            get
            {
                return _withdrawRequestRepository = _withdrawRequestRepository ?? new WithdrawRequestRepository(_dbContext);
            }
        }

        public ICommissionFeeRepository CommissionFeeRepository
        {
            get
            {
                return _commissionFeeRepository = _commissionFeeRepository ?? new CommissionFeeRepository(_dbContext);
            }
        }

        public ILikeRepository likeRepository
        {
            get
            {
                return _likeRepository = _likeRepository ?? new LikeRepository(_mongoDBContext);
            }
        }

        public ICommentRepository commentRepository {
            get
            {
                return _commentRepository = _commentRepository ?? new CommentRepository(_mongoDBContext);
            }
        }

        public IProjectCouponRepository ProjectCouponRepository =>
       _projectCouponRepository ??= new ProjectCouponRepository(_dbContext);

        public IMilestoneRepository MilestoneRepository =>
            _milestoneRepository ??= new MilestoneRepository(_dbContext);

        public IProjectMilestoneRepository ProjectMilestoneRepository =>
            _projectMilestoneRepository ??= new ProjectMilestoneRepository(_dbContext);

        public IRequirementRepository RequirementRepository =>
            _requirementRepository ??= new RequirementRepository(_dbContext);

        public IProjectMilestoneRequirementRepository ProjectMilestoneRequirementRepository =>
            _projectMilestoneRequirementRepository ??= new ProjectMilestoneRequirementRepository(_dbContext);

        public IProjectRequirementFileRepository ProjectRequirementFileRepository =>
            _projectRequirementFileRepository ??= new ProjectRequirementFileRepository(_dbContext);

        public IProjectMilestoneBackerRepository ProjectMilestoneBackerRepository =>
            _projectMilestoneBackerRepository ??= new ProjectMilestoneBackerRepository(_dbContext);


        // Commit and rollback methods
        public void Commit()
             => _dbContext.SaveChanges();

        public async Task CommitAsync()
            => await _dbContext.SaveChangesAsync();

        public void Rollback()
            => _dbContext.Dispose();

        public async Task RollbackAsync()
            => await _dbContext.DisposeAsync();
    }

}
