using AutoMapper;
using Azure;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.CommissionDTO;
using Fun_Funding.Application.ViewModel.WithdrawDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class WithdrawService : IWithdrawService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public WithdrawService(IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _claimsPrincipal = httpContextAccessor.HttpContext.User;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<ResultDTO<WithdrawRequest>> AdminApproveRequest(Guid id)
        {
            var admin = await _userService.GetUserInfo();
            if (admin is null)
            {
                return ResultDTO<WithdrawRequest>.Fail("admin is not found");
            }
            var request = await _unitOfWork.WithdrawRequestRepository.GetByIdAsync(id);
            if (request is null)
            {
                return ResultDTO<WithdrawRequest>.Fail("request not found");
            }
            // status not valid
            if (!request.Status.Equals(WithdrawRequestStatus.Processing) || request.IsFinished)
            {
                return ResultDTO<WithdrawRequest>.Fail("request status is invalid");
            }
            //expired date
            if (request.ExpiredDate < DateTime.UtcNow)
            {
                return ResultDTO<WithdrawRequest>.Fail("request is out of date");
            }
            try
            {
                // change status
                request.Status = WithdrawRequestStatus.Approved;
                request.IsFinished = true;
                _unitOfWork.WithdrawRequestRepository.Update(request);

                Transaction transaction = new Transaction
                {
                    Id = new Guid(),
                    TotalAmount = -request.Amount,
                    Description = $"Admin just APPROVED withdraw request id: {request.Id} with amount: {request.Amount}",
                    CreatedDate = DateTime.UtcNow,
                    WalletId = request.WalletId,
                    TransactionType = TransactionTypes.FundingWithdraw,

                };
                await _unitOfWork.TransactionRepository.AddAsync(transaction);


                await _unitOfWork.CommitAsync();
                return ResultDTO<WithdrawRequest>.Success(request, "Successfully approved this request");

            }
            catch (Exception ex)
            {
                return ResultDTO<WithdrawRequest>.Fail($"Error: {ex.Message}");
            }
        }

        public async Task<ResultDTO<WithdrawRequest>> AdminCancelRequest(Guid id)
        {
            var admin = await _userService.GetUserInfo();
            var request = await _unitOfWork.WithdrawRequestRepository.GetByIdAsync(id);
            if (request is null)
            {
                return ResultDTO<WithdrawRequest>.Fail("request not found");
            }
            // status not valid
            if (!request.Status.Equals(WithdrawRequestStatus.Processing) || request.IsFinished)
            {
                return ResultDTO<WithdrawRequest>.Fail("request status is invalid");
            }
            //expired date
            if (request.ExpiredDate < DateTime.UtcNow)
            {
                return ResultDTO<WithdrawRequest>.Fail("request is out of date");
            }
            try
            {
                // change status
                request.Status = WithdrawRequestStatus.Rejected;
                request.IsFinished = true;
                _unitOfWork.WithdrawRequestRepository.Update(request);

                //transfer back to wallet 
                var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(request.WalletId);
                wallet.Balance += request.Amount; 
                _unitOfWork.WalletRepository.Update(wallet);

                Transaction transaction = new Transaction
                {
                    Id = new Guid(),
                    TotalAmount = request.Amount,
                    Description = $"Admin id: {admin._data.Id} just CANCEL withdraw id: {request.Id}",
                    CreatedDate = DateTime.UtcNow,
                    TransactionType = TransactionTypes.FundingWithdraw,
                };
                await _unitOfWork.TransactionRepository.AddAsync(transaction);


                await _unitOfWork.CommitAsync();
                return ResultDTO<WithdrawRequest>.Success(request, "Successfully cancel this request");

            }
            catch (Exception ex)
            {
                return ResultDTO<WithdrawRequest>.Fail($"Error: {ex.Message}");
            }
        }

        public async Task<ResultDTO<AdminResponse>> AdminProcessingRequest(Guid id)
        {
            var admin = await _userService.GetUserInfo();
            if (admin is null)
            {
                return ResultDTO<AdminResponse>.Fail("admin not found");
            }
            var request = await _unitOfWork.WithdrawRequestRepository.GetByIdAsync(id);
            if (request is null)
            {
                return ResultDTO<AdminResponse>.Fail("request not found");
            }
            // status not valid
            if (!request.Status.Equals(WithdrawRequestStatus.Pending) || request.IsFinished)
            {
                return ResultDTO<AdminResponse>.Fail("request status is invalid");
            }
            //expired date
            if (request.ExpiredDate < DateTime.UtcNow)
            {
                return ResultDTO<AdminResponse>.Fail("request is out of date");
            }
            try
            {
                // change status
                request.Status = WithdrawRequestStatus.Processing;
                _unitOfWork.WithdrawRequestRepository.Update(request);

                //get BankAccount
                var bankAccount = await _unitOfWork.BankAccountRepository.GetAsync(x => x.Wallet!.Id.Equals(request.WalletId));
                await _unitOfWork.CommitAsync();
                AdminResponse response = new AdminResponse
                {
                    AdminId = admin._data.Id,
                    BankCode = bankAccount.BankCode,
                    BankNumber = bankAccount.BankNumber,
                    WithdrawRequest = request,
                };
                return ResultDTO<AdminResponse>.Success(response, "Successfully processing this request");

            }
            catch (Exception ex)
            {
                return ResultDTO<AdminResponse>.Fail($"Error: {ex.Message}");
            }
        }

        public async Task<ResultDTO<PaginatedResponse<WithdrawRequest>>> GetAllRequest(ListRequest request)
        {

            try
            {
                Expression<Func<WithdrawRequest, bool>> filter = null;
                Expression<Func<WithdrawRequest, object>> orderBy = c => c.ExpiredDate;

                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    filter = c => c.Status.ToString().Contains(request.SearchValue.ToLower());
                }

                var list = await _unitOfWork.WithdrawRequestRepository.GetAllAsync(
                    filter: filter,
                    orderBy: orderBy,
                    isAscending: request.IsAscending.Value,
                    pageIndex: request.PageIndex,
                    pageSize: request.PageSize
                    );
                if (list != null)
                {
                    var totalItems = _unitOfWork.WithdrawRequestRepository.GetAll(filter).Count();
                    var totalPages = (int)Math.Ceiling((double)totalItems / (int)request.PageSize);

                    

                    PaginatedResponse<WithdrawRequest> response = new PaginatedResponse<WithdrawRequest>
                    {
                        PageSize = request.PageSize.Value,
                        PageIndex = request.PageIndex.Value,
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        Items = list
                    };

                    return ResultDTO<PaginatedResponse<WithdrawRequest>>.Success(response,"Successfull");
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Commission Fee Not Found.");
                }

            }
            catch (Exception ex)
            {
                return ResultDTO<PaginatedResponse<WithdrawRequest>>.Fail($"error: {ex.Message}");
            }
        }

        public async Task<ResultDTO<WithdrawResponse>> CreateMarketplaceRequest(Guid MarketplaceId)
        {
            // Check User
            var user = await _userService.GetUserInfo();
            if (user._data == null)
            {
                return ResultDTO<WithdrawResponse>.Fail("UserID can not be null");
            }
            // Check Request
            if (MarketplaceId == null)
            {
                return ResultDTO<WithdrawResponse>.Fail("ProjectID can not be null");
            }
            var marketplaceProject = await _unitOfWork.MarketplaceRepository.GetByIdAsync(MarketplaceId);
            if (marketplaceProject == null)
            {
                return ResultDTO<WithdrawResponse>.Fail("Project can not be null");
            }
            //get markeplace wallet
            var marketplaceWallet = await _unitOfWork.WalletRepository.GetAsync(x => x.MarketplaceProject!.Id == MarketplaceId);
            if (marketplaceWallet == null)
            {
                return ResultDTO<WithdrawResponse>.Fail("Waller can not found");
            }
            if (marketplaceWallet.Balance < 10000)
            {
                return ResultDTO<WithdrawResponse>.Fail("Your account balance must be higher than 10.000 VND to withdraw balance.", (int)HttpStatusCode.Forbidden);
            }
            
            _unitOfWork.WalletRepository.Update(marketplaceWallet);
            // Check exited Request
            var exitedRequest = _unitOfWork.WithdrawRequestRepository.GetQueryable()
                .Where(x => x.WalletId.Equals(marketplaceWallet.Id))
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault();

            if (exitedRequest != null)
            {
                // Check if the existing request is not finished
                if (!exitedRequest.IsFinished)
                {
                    return ResultDTO<WithdrawResponse>.Fail("This project already has a request. You must wait for the review to complete.");
                }
            }
            try
            {
                WithdrawRequest withdrawRequest = new WithdrawRequest
                {
                    Id = new Guid(),
                    Amount = marketplaceWallet.Balance,
                    CreatedDate = DateTime.UtcNow,
                    ExpiredDate = DateTime.UtcNow.AddDays(7),
                    IsFinished = false,
                    WalletId = marketplaceWallet.Id,
                    RequestType = TransactionTypes.MarketplaceWithdraw,
                    Status = WithdrawRequestStatus.Pending,
                };
                await _unitOfWork.WithdrawRequestRepository.AddAsync(withdrawRequest);

                //update money in wallet marketplace 
                marketplaceWallet.Balance = 0;

                await _unitOfWork.CommitAsync();
                WithdrawResponse response = _mapper.Map<WithdrawResponse>(withdrawRequest);
                return ResultDTO<WithdrawResponse>.Success(response, "Your withdraw has been create, please wait for admin to review");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResultDTO<string>> WalletWithdrawRequest(decimal amount)
        {
            try
            {
                if (_claimsPrincipal == null || !_claimsPrincipal.Identity.IsAuthenticated)
                {
                    throw new ExceptionError((int)HttpStatusCode.Unauthorized, "User not authenticated.");
                }
                var userEmailClaims = _claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (userEmailClaims == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }
                var userEmail = userEmailClaims.Value;
                var user = await _unitOfWork.UserRepository.GetQueryable()
                                .AsNoTracking()
                                .Include(u => u.Wallet)
                                .FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }
                Wallet? wallet = user.Wallet;
                if (wallet.Balance < 10000)
                {
                    return ResultDTO<string>.Fail("Your account balance must be higher than 10.000 VND to withdraw balance.", (int)HttpStatusCode.Forbidden);
                }
                else if (amount <= 10000)
                {
                    return ResultDTO<string>.Fail("Must withdraw at least 10.000 VND to withdraw balance.", (int)HttpStatusCode.Forbidden);
                }
                else if (wallet.Balance < amount)
                {
                    return ResultDTO<string>.Fail("Not enough money to withdraw balance.", (int)HttpStatusCode.Forbidden);
                }
                wallet.Balance -= amount;
                _unitOfWork.WalletRepository.Update(wallet);

                WithdrawRequest withdrawRequest = new WithdrawRequest
                {
                    Id = new Guid(),
                    Amount = amount,
                    CreatedDate = DateTime.UtcNow,
                    ExpiredDate = DateTime.UtcNow.AddDays(7),
                    IsFinished = false,
                    WalletId = wallet.Id,
                    RequestType = TransactionTypes.WithdrawWalletMoney,
                    Status = WithdrawRequestStatus.Pending,
                };
                await _unitOfWork.WithdrawRequestRepository.AddAsync(withdrawRequest);
                await _unitOfWork.CommitAsync();
                return ResultDTO<string>.Success("", "Your withdraw has been create, please wait for admin to review");
            }
            catch (Exception ex)
            {
                if (ex is ExceptionError exceptionError)
                {
                    throw exceptionError;
                }
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResultDTO<WithdrawRequest>> GetWithdrawRequestById(Guid Id)
        {
            try
            {
                var request = await _unitOfWork.WithdrawRequestRepository.GetByIdAsync(Id);
                if (request is null)
                {
                    return ResultDTO<WithdrawRequest>.Fail("no request found");
                }
                return ResultDTO<WithdrawRequest>.Success(request, "Successfully find request");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
