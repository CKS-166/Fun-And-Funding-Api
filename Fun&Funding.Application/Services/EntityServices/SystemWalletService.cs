using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.Interfaces.IEntityService;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class SystemWalletService : ISystemWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public SystemWalletService(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
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
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<object>> GetDashboardFundingProjects()
        {
            try
            {
                ProjectStatus[] statuses =
                {
                    ProjectStatus.FundedSuccessful,
                    ProjectStatus.Processing,
                    ProjectStatus.Successful,
                    ProjectStatus.Failed,
                    ProjectStatus.Reported
                };

                var response = new List<object>();

                foreach (var status in statuses)
                {
                    Expression<Func<FundingProject, bool>> filter = p => p.Status == status;

                    var fundingProjects = await _unitOfWork.FundingProjectRepository.GetAllAsync(filter);

                    response.Add(new
                    {
                        Status = status.ToString(),
                        Count = fundingProjects.Count()
                    });
                }

                return ResultDTO<object>.Success(response);
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

        public async Task<ResultDTO<object>> GetDashboardMarketplaceProjects()
        {
            try
            {
                ProjectStatus[] statuses =
                {
                    ProjectStatus.Processing,
                    ProjectStatus.Reported
                };

                var response = new List<object>();

                foreach (var status in statuses)
                {
                    Expression<Func<MarketplaceProject, bool>> filter = p => p.Status == status;

                    var marketplaceProjects = await _unitOfWork.MarketplaceRepository.GetAllAsync(filter);

                    response.Add(new
                    {
                        Status = status.ToString(),
                        Count = marketplaceProjects.Count()
                    });
                }

                return ResultDTO<object>.Success(response);
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

        public async Task<ResultDTO<object>> GetDashboardMetrics()
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetQueryable().AsNoTracking().ToListAsync();
                var fundingProjects = await _unitOfWork.FundingProjectRepository.GetAllAsync();
                var marketplaceProjects = await _unitOfWork.MarketplaceRepository.GetAllAsync();

                var response = new
                {
                    NumberOfUsers = users?.Count ?? 0,
                    NumberOfFundingProjects = fundingProjects?.Count() ?? 0,
                    NumberOfMarketplaceProjects = marketplaceProjects?.Count() ?? 0,
                };

                return ResultDTO<object>.Success(response);
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

        public async Task<ResultDTO<object>> GetDashboardMilestones()
        {
            try
            {
                var milestoneOrders = await _unitOfWork.MilestoneRepository
                    .GetQueryable()
                    .AsNoTracking()
                    .GroupBy(m => m.MilestoneOrder)
                    .Select(m => m.Key)
                    .ToListAsync();

                var response = new List<object>();

                foreach (var order in milestoneOrders)
                {
                    Expression<Func<ProjectMilestone, bool>> filter = p => p.Milestone.MilestoneOrder == order;

                    var projectMilestones = await _unitOfWork.ProjectMilestoneRepository
                        .GetQueryable()
                        .AsNoTracking()
                        .Where(filter)
                        .ToListAsync();

                    response.Add(new
                    {
                        MilestoneOrder = order,
                        Count = projectMilestones.Count()
                    });
                }

                return ResultDTO<object>.Success(response);
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

        public async Task<ResultDTO<object>> GetDashboardUsers()
        {
            try
            {
                var backers = await _userService.GetUsersByRoleAsync("Backer");
                var gameOwners = await _userService.GetUsersByRoleAsync("GameOwner");

                var response = new[]
                {
                    new { Role = "Backer", Count = backers?.Count ?? 0 },
                    new { Role = "GameOwner", Count = gameOwners?.Count ?? 0 }
                };

                return ResultDTO<object>.Success(response);
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

        public async Task<ResultDTO<decimal>> GetPlatformRevenue()
        {
            try
            {
                var systemWallet = await _unitOfWork.SystemWalletRepository.GetQueryable().SingleOrDefaultAsync();
                if (systemWallet == null)
                {
                    ResultDTO<decimal>.Success(0, "Platform balance");
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
