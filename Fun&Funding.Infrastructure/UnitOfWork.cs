using Fun_Funding.Application;
using Fun_Funding.Application.IRepository;
using Fun_Funding.Infrastructure.Database;
using Fun_Funding.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyDbContext _dbContext;

        // Repositories
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

        public UnitOfWork(MyDbContext dbContext)
        {
            _dbContext = dbContext;
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
