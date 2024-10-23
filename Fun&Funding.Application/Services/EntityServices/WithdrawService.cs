using AutoMapper;
using Azure;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.WithdrawDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        //public async Task<ResultDTO<WithdrawRequest>> AdminApproveRequest(Guid id)
        //{
        //    var admin = await _userService.GetUserInfo();
        //    var request = await _unitOfWork.WithdrawRequestRepository.GetByIdAsync(id);
        //    if (request is null)
        //    {
        //        return ResultDTO<WithdrawRequest>.Fail("request not found");
        //    }
        //    // status not valid
        //    if (!request.Status.Equals(WithdrawRequestStatus.Processing) || request.IsFinished)
        //    {
        //        return ResultDTO<WithdrawRequest>.Fail("request status is invalid");
        //    }
        //    //expired date
        //    if (request.ExpiredDate < DateTime.UtcNow)
        //    {
        //        return ResultDTO<WithdrawRequest>.Fail("request is out of date");
        //    }
        //    try
        //    {
        //        // change status
        //        request.Status = WithdrawRequestStatus.Approved;
        //        request.IsFinished = true;
        //        _unitOfWork.WithdrawRequestRepository.Update(request);

        //        Transaction transaction = new Transaction
        //        {
        //            Id = new Guid(),
        //            TotalAmount = request.Amount,
        //            Description = $"Admin id: {admin._data.Id} just APPROVED withdraw id: {request.Id} with amount: {request.Amount}",
        //            CreatedDate = DateTime.UtcNow,
        //            TransactionType = TransactionTypes.FundingWithdraw,

        //        };
        //        await _unitOfWork.TransactionRepository.AddAsync(transaction);


        //        await _unitOfWork.CommitAsync();
        //        return ResultDTO<WithdrawRequest>.Success(request, "Successfully approved this request");

        //    }
        //    catch (Exception ex)
        //    {
        //        return ResultDTO<WithdrawRequest>.Fail($"Error: {ex.Message}");
        //    }
        //}

        //public async Task<ResultDTO<WithdrawRequest>> AdminCancelRequest(Guid id)
        //{
        //    var admin = await _userService.GetUserInfo();
        //    var request = await _unitOfWork.WithdrawRequestRepository.GetByIdAsync(id);
        //    if (request is null)
        //    {
        //        return ResultDTO<WithdrawRequest>.Fail("request not found");
        //    }
        //    // status not valid
        //    if (!request.Status.Equals(WithdrawRequestStatus.Processing) || request.IsFinished)
        //    {
        //        return ResultDTO<WithdrawRequest>.Fail("request status is invalid");
        //    }
        //    //expired date
        //    if (request.ExpiredDate < DateTime.UtcNow)
        //    {
        //        return ResultDTO<WithdrawRequest>.Fail("request is out of date");
        //    }
        //    try
        //    {
        //        // change status
        //        request.Status = WithdrawRequestStatus.Rejected;
        //        request.IsFinished = true;
        //        _unitOfWork.WithdrawRequestRepository.Update(request);

        //        Transaction transaction = new Transaction
        //        {
        //            Id = new Guid(),
        //            TotalAmount = request.Amount,
        //            Description = $"Admin id: {admin._data.Id} just CANCEL withdraw id: {request.Id}",
        //            CreatedDate = DateTime.UtcNow,
        //            TransactionType = TransactionTypes.FundingWithdraw,

        //        };
        //        await _unitOfWork.TransactionRepository.AddAsync(transaction);


        //        await _unitOfWork.CommitAsync();
        //        return ResultDTO<WithdrawRequest>.Success(request, "Successfully cancel this request");

        //    }
        //    catch (Exception ex)
        //    {
        //        return ResultDTO<WithdrawRequest>.Fail($"Error: {ex.Message}");
        //    }
        //}

        //public async Task<ResultDTO<AdminResponse>> AdminProcessingRequest(Guid id)
        //{
        //    var admin = await _userService.GetUserInfo();
        //    var request = await _unitOfWork.WithdrawRequestRepository.GetByIdAsync(id);
        //    if (request is null)
        //    {
        //        return ResultDTO<AdminResponse>.Fail("request not found");
        //    }
        //    // status not valid
        //    if (!request.Status.Equals(WithdrawRequestStatus.Pending) || request.IsFinished)
        //    {
        //        return ResultDTO<AdminResponse>.Fail("request status is invalid");
        //    }
        //    //expired date
        //    if (request.ExpiredDate < DateTime.UtcNow)
        //    {
        //        return ResultDTO<AdminResponse>.Fail("request is out of date");
        //    }
        //    try
        //    {
        //        // change status
        //        request.Status = WithdrawRequestStatus.Processing;
        //        _unitOfWork.WithdrawRequestRepository.Update(request);

        //        //get BankAccount
        //        var project = await _unitOfWork.FundingProjectRepository.GetQueryable()
        //            .Include(x => x.BankAccount)
        //            .FirstOrDefaultAsync(x => x.Id.Equals(request.ProjectId));
        //        var bankAccount = await _unitOfWork.BankAccountRepository.GetByIdAsync(project.BankAccount.Id);
        //        await _unitOfWork.CommitAsync();
        //        AdminResponse response = new AdminResponse
        //        {
        //            AdminId = admin._data.Id,
        //            BankCode = bankAccount.BankCode,
        //            BankNumber = bankAccount.BankNumber,
        //            WithdrawRequest = request,
        //        };
        //        return ResultDTO<AdminResponse>.Success(response, "Successfully processing this request");

        //    }
        //    catch (Exception ex)
        //    {
        //        return ResultDTO<AdminResponse>.Fail($"Error: {ex.Message}");
        //    }
        //}

        //public async Task<ResultDTO<List<WithdrawRequest>>> GetAllRequest()
        //{
        //    try
        //    {
        //        var list = await _unitOfWork.WithdrawRequestRepository.GetAllAsync();
        //        return ResultDTO<List<WithdrawRequest>>.Success(list.ToList(), "list of withdraw request");

        //    }
        //    catch (Exception ex)
        //    {
        //        return ResultDTO<List<WithdrawRequest>>.Fail($"error: {ex.Message}");
        //    }
        //}

        //public async Task<ResultDTO<WithdrawResponse>> OwnerCreateRequest(WithdrawReq request)
        //{
        //    // Check User
        //    var user = await _userService.GetUserInfo();
        //    if (user._data == null)
        //    {
        //        return ResultDTO<WithdrawResponse>.Fail("UserID can not be null");
        //    }
        //    // Check Request
        //    if (request == null)
        //    {
        //        return ResultDTO<WithdrawResponse>.Fail("ProjectID can not be null");
        //    }
        //    var fundingProject = await _unitOfWork.FundingProjectRepository.GetByIdAsync(request.ProjectId);
        //    if (fundingProject == null)
        //    {
        //        return ResultDTO<WithdrawResponse>.Fail("Project can not be null");
        //    }
        //    // Check Project match user id
        //    if (!fundingProject.UserId.Equals(user._data.Id))
        //    {
        //        return ResultDTO<WithdrawResponse>.Fail("This User do not have same valid projectId");
        //    }
        //    // CheckFundingProjectStatus
        //    if (!fundingProject.Status.Equals(ProjectStatus.Successful))
        //    {
        //        return ResultDTO<WithdrawResponse>.Fail("Project is not in valid status, required: Successfull");
        //    }
        //    // Check exited Request
        //    var exitedRequest = await _unitOfWork.WithdrawRequestRepository.GetAsync(x => x.ProjectId == request.ProjectId);
        //    if (exitedRequest != null)
        //    {
        //        return ResultDTO<WithdrawResponse>.Fail("This Project already have request, you can only request once per 7 day");
        //    }
        //    try
        //    {
        //        WithdrawRequest withdrawRequest = new WithdrawRequest
        //        {
        //            Id = new Guid(),
        //            Amount = fundingProject.Balance,
        //            CreatedDate = DateTime.UtcNow,
        //            ExpiredDate = DateTime.UtcNow.AddDays(7),
        //            IsFinished = false,
        //            ProjectId = request.ProjectId,
        //            RequestType = TransactionTypes.FundingWithdraw,
        //            Status = WithdrawRequestStatus.Pending,
        //        };
        //        await _unitOfWork.WithdrawRequestRepository.AddAsync(withdrawRequest);
        //        await _unitOfWork.CommitAsync();
        //        WithdrawResponse response = new WithdrawResponse
        //        {
        //            Amount = withdrawRequest.Amount,
        //            ExpiredDate = withdrawRequest.ExpiredDate,
        //            IsFinished = false,
        //            ProjectId = request.ProjectId,
        //            RequestType = TransactionTypes.FundingWithdraw,
        //            Status = WithdrawRequestStatus.Pending,
        //        };
        //        return ResultDTO<WithdrawResponse>.Success(response, "Your withdraw has been create, please wait for admin to review");

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        //public async Task<ResultDTO<string>> WalletWithdrawRequest()
        //{
        //    try
        //    {
        //        if (_claimsPrincipal == null || !_claimsPrincipal.Identity.IsAuthenticated)
        //        {
        //            throw new ExceptionError((int)HttpStatusCode.Unauthorized, "User not authenticated.");
        //        }
        //        var userEmailClaims = _claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        //        if (userEmailClaims == null)
        //        {
        //            throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
        //        }
        //        var userEmail = userEmailClaims.Value;
        //        var user = await _unitOfWork.UserRepository.GetQueryable()
        //                        .AsNoTracking()
        //                        .Include(u => u.Wallet)
        //                        .FirstOrDefaultAsync(u => u.Email == userEmail);
        //        if (user == null)
        //        {
        //            throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
        //        }
        //        Wallet? wallet = user.Wallet;
        //        if (wallet.Balance < 10000)
        //        {
        //            return ResultDTO<string>.Fail("Your account balance must be higher than 10.000 VND to withdraw balance.", (int)HttpStatusCode.Forbidden);
        //        }
        //        WithdrawRequest withdrawRequest = new WithdrawRequest
        //        {
        //            Id = new Guid(),
        //            Amount = wallet.Balance,
        //            CreatedDate = DateTime.UtcNow,
        //            ExpiredDate = DateTime.UtcNow.AddDays(7),
        //            IsFinished = false,
        //            WalletId = wallet.Id,
        //            RequestType = TransactionTypes.WithdrawWalletMoney,
        //            Status = WithdrawRequestStatus.Pending,
        //        };
        //        await _unitOfWork.WithdrawRequestRepository.AddAsync(withdrawRequest);
        //        await _unitOfWork.CommitAsync();
        //        return ResultDTO<string>.Success("", "Your withdraw has been create, please wait for admin to review");
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex is ExceptionError exceptionError)
        //        {
        //            throw exceptionError;
        //        }
        //        throw new Exception(ex.Message);
        //    }
        //}
    }
}
