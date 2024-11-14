﻿using AutoMapper;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.Interfaces.IExternalServices;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.BankAccountDTO;
using Fun_Funding.Application.ViewModel.MarketplaceFileDTO;
using Fun_Funding.Application.ViewModel.MarketplaceProjectDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class MarketplaceService : IMarketplaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IFundingProjectService _fundingProjectService;
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly IAzureService _azureService;

        public MarketplaceService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService,
            IHttpContextAccessor httpContextAccessor, IFundingProjectService fundingProjectService,
            IAzureService azureService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
            _claimsPrincipal = httpContextAccessor.HttpContext.User;
            _fundingProjectService = fundingProjectService;
            _azureService = azureService;
        }

        public async Task<ResultDTO<MarketplaceProjectInfoResponse>>
            CreateMarketplaceProject(MarketplaceProjectAddRequest request)
        {
            try
            {
                //check if marketplace project is deleted
                var mp = await _unitOfWork.MarketplaceRepository
                    .GetQueryable()
                    .Where(p => p.FundingProjectId == request.FundingProjectId)
                    .Include(p => p.Wallet)
                    .ThenInclude(p => p.BankAccount)
                    .FirstOrDefaultAsync();

                if (mp != null && (mp.Status == ProjectStatus.Deleted || mp.IsDeleted == true))
                {
                    var updateRequest = _mapper.Map<MarketplaceProjectUpdateRequest>(request);

                    //map bank account
                    BankAccountUpdateRequest bankAccount = new BankAccountUpdateRequest
                    {
                        Id = mp.Wallet.BankAccount.Id,
                        BankCode = request.BankAccount.BankCode,
                        BankNumber = request.BankAccount.BankNumber
                    };
                    updateRequest.BankAccount = bankAccount;

                    var response = UpdateMarketplaceProject(mp.Id, updateRequest, true).Result._data;

                    return new ResultDTO<MarketplaceProjectInfoResponse>(true, "Create successfully.",
                        response, (int)HttpStatusCode.Created);
                }
                else
                {
                    //find funding project
                    var fundingProject = await _unitOfWork.FundingProjectRepository.GetQueryable()
                        .Where(p => p.Id == request.FundingProjectId)
                        .Include(p => p.User)
                        .Include(p => p.Categories)
                        .Include(p => p.MarketplaceProject)
                        .FirstOrDefaultAsync();

                    if (fundingProject == null)
                        throw new ExceptionError((int)HttpStatusCode.NotFound, "Funding Project not found.");
                    else if (fundingProject.Status != ProjectStatus.Successful)
                        throw new ExceptionError((int)HttpStatusCode.BadRequest
                            , "The project cannot be published to marketplace if it has not complete crowdfunding on Fun&Funding platform.");
                    else if (fundingProject.MarketplaceProject != null)
                        throw new ExceptionError((int)HttpStatusCode.BadRequest,
                            "There is already a project promoted to the marketplace from the same funding project.");
                    else
                    {
                        //validate
                        var errorMessages = ValidateMarketplaceProject(request);
                        if (errorMessages != null && errorMessages.Count > 0)
                        {
                            throw new ExceptionError((int)HttpStatusCode.BadRequest, string.Join("\n", errorMessages));
                        }

                        //add files 
                        List<MarketplaceFile> files = new List<MarketplaceFile>();

                        foreach (MarketplaceFileRequest file in request.MarketplaceFiles)
                        {
                            if (file.URL.Length > 0)
                            {
                                var result = _azureService.UploadUrlSingleFiles(file.URL);

                                if (result == null)
                                {
                                    throw new ExceptionError((int)HttpStatusCode.BadRequest, "Fail to upload file");
                                }

                                MarketplaceFile media = new MarketplaceFile
                                {
                                    Name = file.Name,
                                    URL = result.Result,
                                    FileType = file.FileType,
                                    CreatedDate = DateTime.Now
                                };

                                if (file.FileType == FileType.GameFile)
                                {
                                    media.Version = "1";
                                    media.Description = "First upload";
                                }

                                files.Add(media);
                            }
                        }

                        //map project
                        var marketplaceProject = _mapper.Map<MarketplaceProject>(request);
                        marketplaceProject.MarketplaceFiles = files;
                        marketplaceProject.FundingProject = fundingProject;
                        marketplaceProject.Status = ProjectStatus.Pending;
                        marketplaceProject.CreatedDate = DateTime.Now;

                        //create a wallet
                        Wallet wallet = new Wallet
                        {
                            MarketplaceProject = marketplaceProject,
                            Balance = 0,
                            CreatedDate = DateTime.Now
                        };

                        //bank account for wallet
                        BankAccount bankAccount = new BankAccount
                        {
                            Wallet = wallet,
                            BankCode = request.BankAccount.BankCode,
                            BankNumber = request.BankAccount.BankNumber,
                            CreatedDate = DateTime.Now
                        };

                        marketplaceProject.Wallet = wallet;
                        marketplaceProject.Wallet.BankAccount = bankAccount;

                        //save to db
                        await _unitOfWork.MarketplaceRepository.AddAsync(marketplaceProject);
                        await _unitOfWork.WalletRepository.AddAsync(wallet);
                        await _unitOfWork.BankAccountRepository.AddAsync(bankAccount);

                        await _unitOfWork.CommitAsync();

                        //response
                        var response = _mapper.Map<MarketplaceProjectInfoResponse>(marketplaceProject);
                        return new ResultDTO<MarketplaceProjectInfoResponse>(true, "Create successfully.",
                            response, (int)HttpStatusCode.Created);
                    }
                }
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

        public async Task<ResultDTO<PaginatedResponse<MarketplaceProject>>> GetAllMarketplaceProject(ListRequest request)
        {

            try
            {
                Expression<Func<MarketplaceProject, bool>> filter = null;
                Expression<Func<MarketplaceProject, object>> orderBy = c => c.CreatedDate;

                if (!string.IsNullOrEmpty(request.OrderBy))
                {
                    switch (request.OrderBy.ToLower())
                    {
                        case "name":
                            orderBy = c => c.Name;
                            break;
                        default:
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    filter = c => c.Name.ToLower().Contains(request.SearchValue.ToLower());
                }

                var list = await _unitOfWork.MarketplaceRepository.GetAllAsync(
                   filter: filter,
                   orderBy: orderBy,
                   isAscending: request.IsAscending.Value,
                   pageIndex: request.PageIndex,
                   pageSize: request.PageSize);

                if (list != null && list.Count() > 0)
                {
                    var totalItems = _unitOfWork.MarketplaceRepository.GetAll(filter).Count();
                    var totalPages = (int)Math.Ceiling((double)totalItems / (int)request.PageSize);
                    IEnumerable<MarketplaceProject> marketplaceProjects = _mapper.Map<IEnumerable<MarketplaceProject>>(list);

                    PaginatedResponse<MarketplaceProject> response = new PaginatedResponse<MarketplaceProject>
                    {
                        PageSize = request.PageSize.Value,
                        PageIndex = request.PageIndex.Value,
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        Items = marketplaceProjects
                    };

                    return ResultDTO<PaginatedResponse<MarketplaceProject>>.Success(response);
                }
                else
                {
                    return ResultDTO<PaginatedResponse<MarketplaceProject>>.Fail("Marketplace Not Found");
                }

            }
            catch (Exception ex)
            {
                return ResultDTO<PaginatedResponse<MarketplaceProject>>.Fail("Something wrong");
            }
        }

        public async Task<ResultDTO<MarketplaceProjectInfoResponse>> GetMarketplaceProjectById(Guid id)
        {
            try
            {
                var marketplaceProject = await _unitOfWork.MarketplaceRepository.GetQueryable()
                    .Where(p => p.Id == id && p.IsDeleted == false)
                    .Include(p => p.MarketplaceFiles)
                    .Include(p => p.FundingProject.Categories)
                    .Include(p => p.FundingProject)
                    .ThenInclude(p => p.User)
                    .Include(p => p.Wallet)
                    .ThenInclude(p => p.BankAccount)
                    .FirstOrDefaultAsync();

                if (marketplaceProject == null)
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Marketplace Project not found.");
                else
                {
                    if (marketplaceProject.MarketplaceFiles != null)
                    {
                        var existingFiles = marketplaceProject.MarketplaceFiles.ToList();
                        marketplaceProject.MarketplaceFiles = GetNonDeletedFiles(existingFiles);
                    }

                    var response = _mapper.Map<MarketplaceProjectInfoResponse>(marketplaceProject);

                    return ResultDTO<MarketplaceProjectInfoResponse>.Success(response);
                }
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

        public async Task DeleteMarketplaceProject(Guid id)
        {
            try
            {
                var marketPlaceProject = await _unitOfWork.MarketplaceRepository
                    .GetQueryable()
                    .Where(p => p.Id == id)
                    .Include(p => p.MarketplaceFiles)
                    .Include(p => p.ProjectCoupons)
                    .Include(p => p.Wallet)
                    .ThenInclude(p => p.BankAccount)
                    .FirstOrDefaultAsync();

                if (marketPlaceProject == null)
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Marketplace Project not found.");
                else if (marketPlaceProject.Status != ProjectStatus.Pending)
                    throw new ExceptionError((int)HttpStatusCode.BadRequest, "Marketplace Project cannot be deleted.");
                else
                {
                    //remove related files
                    if (marketPlaceProject.MarketplaceFiles != null
                        && marketPlaceProject.MarketplaceFiles.Count > 0)
                    {
                        _unitOfWork.MarketplaceFileRepository.RemoveRange(marketPlaceProject.MarketplaceFiles);
                    }

                    //remove related coupons
                    if (marketPlaceProject.ProjectCoupons != null
                        && marketPlaceProject.ProjectCoupons.Count > 0)
                    {
                        _unitOfWork.ProjectCouponRepository.RemoveRange(marketPlaceProject.ProjectCoupons);
                    }

                    //remove related wallet
                    var wallet = marketPlaceProject.Wallet;

                    if (wallet != null)
                    {
                        _unitOfWork.WalletRepository.Remove(wallet);

                        //remove related bank account
                        var bankAccount = marketPlaceProject.Wallet.BankAccount;

                        if (bankAccount != null) _unitOfWork.BankAccountRepository.Remove(bankAccount);
                    }

                    _unitOfWork.MarketplaceRepository.Remove(marketPlaceProject);
                    await _unitOfWork.CommitAsync();

                    await UpdateMarketplaceProjectStatus(id, ProjectStatus.Deleted);
                }
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


        public async Task<ResultDTO<MarketplaceProjectInfoResponse>>
            UpdateMarketplaceProject(Guid id, MarketplaceProjectUpdateRequest request, bool? isDeleted = null)
        {
            try
            {
                MarketplaceProject marketplaceProject = await _unitOfWork.MarketplaceRepository
                                        .GetQueryable()
                                        .Where(p => p.Id == id)
                                        .Include(p => p.MarketplaceFiles)
                                        .Include(p => p.FundingProject.Categories)
                                        .Include(p => p.FundingProject)
                                        .ThenInclude(p => p.User)
                                        .Include(p => p.Wallet)
                                        .ThenInclude(p => p.BankAccount)
                                        .FirstOrDefaultAsync();

                if (marketplaceProject == null)
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Marketplace Project not found.");
                else
                {
                    if (isDeleted != null && isDeleted == true)
                    {
                        marketplaceProject.Status = ProjectStatus.Pending;
                        marketplaceProject.IsDeleted = false;
                        marketplaceProject.DeletedAt = null;
                        marketplaceProject.CreatedDate = DateTime.Now;

                        //restore wallet
                        var wallet = marketplaceProject.Wallet;
                        if (wallet != null)
                        {
                            marketplaceProject.Wallet.IsDeleted = false;
                            marketplaceProject.Wallet.DeletedAt = null;
                            marketplaceProject.Wallet.CreatedDate = DateTime.Now;

                            //restore bank account
                            var bankAccount = marketplaceProject.Wallet.BankAccount;
                            if (bankAccount != null)
                            {
                                marketplaceProject.Wallet.BankAccount.IsDeleted = false;
                                marketplaceProject.Wallet.BankAccount.DeletedAt = null;
                                marketplaceProject.Wallet.BankAccount.CreatedDate = DateTime.Now;
                            }
                        }
                    }

                    if (marketplaceProject.Status != ProjectStatus.Deleted
                        && marketplaceProject.Status != ProjectStatus.Reported
                        && marketplaceProject.Status != ProjectStatus.Rejected)
                    {
                        //validate
                        var errorMessages = ValidateMarketplaceProject(request);
                        if (errorMessages != null && errorMessages.Count > 0)
                        {
                            throw new ExceptionError((int)HttpStatusCode.BadRequest, string.Join("\n", errorMessages));
                        }

                        var marketplaceFiles = marketplaceProject.MarketplaceFiles;

                        //remove deleted files
                        var filesToDelete = _mapper.Map<IEnumerable<MarketplaceFile>>(request.ExistingFiles);
                        if (filesToDelete != null && filesToDelete.Count() > 0)
                        {
                            foreach (var file in filesToDelete)
                            {
                                if (file.IsDeleted) _unitOfWork.MarketplaceFileRepository.DeleteMarketplaceFile(file);
                            }
                        }

                        //files to be update
                        var updateFiles = request.MarketplaceFiles;
                        if (updateFiles != null && updateFiles.Count() > 0)
                        {
                            var filesToUpdate = AddFiles(updateFiles, id);

                            marketplaceFiles = marketplaceFiles.Concat(filesToUpdate).ToList();
                        }

                        _mapper.Map(request, marketplaceProject);

                        if (marketplaceProject.Wallet != null)
                        {
                            var bankAccount = marketplaceProject.Wallet.BankAccount;
                            _mapper.Map(request.BankAccount, bankAccount);

                            _unitOfWork.BankAccountRepository.Update(bankAccount);

                            marketplaceProject.Wallet.BankAccount = bankAccount;
                        }

                        marketplaceProject.MarketplaceFiles = marketplaceFiles;

                        _unitOfWork.MarketplaceRepository.Update(marketplaceProject);
                        await _unitOfWork.CommitAsync();

                        //return non-deleted files only
                        marketplaceProject.MarketplaceFiles = GetNonDeletedFiles(marketplaceFiles.ToList());

                        var response = _mapper.Map<MarketplaceProjectInfoResponse>(marketplaceProject);

                        return ResultDTO<MarketplaceProjectInfoResponse>.Success(response);
                    }
                    else
                        throw new ExceptionError((int)HttpStatusCode.BadRequest,
                            $"Marketplace Project cannot be updated when in status {marketplaceProject.Status}.");
                }
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

        public async Task<ResultDTO<MarketplaceProjectInfoResponse>> UpdateMarketplaceProjectStatus(Guid id, ProjectStatus status)
        {
            try
            {
                var marketplaceProject = await _unitOfWork.MarketplaceRepository.GetQueryable()
                    .Where(p => p.Id == id)
                    .Include(p => p.MarketplaceFiles)
                    .Include(p => p.FundingProject.Categories)
                    .Include(p => p.FundingProject)
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync();

                //pending status change list
                List<ProjectStatus> pendingChangelist = new List<ProjectStatus>()
                    {
                        ProjectStatus.Processing,
                        ProjectStatus.Rejected,
                        ProjectStatus.Deleted
                    };

                //rejected status change list
                List<ProjectStatus> rejectedChangelist = new List<ProjectStatus>()
                    {
                        ProjectStatus.Deleted
                    };

                //processing status change list
                List<ProjectStatus> processingChangelist = new List<ProjectStatus>()
                    {
                        ProjectStatus.Reported
                    };

                bool isChanged = false;

                if (marketplaceProject != null)
                {
                    //change status from pending
                    if (marketplaceProject.Status == ProjectStatus.Pending && pendingChangelist.Contains(status))
                    {
                        marketplaceProject.Status = status;
                        isChanged = true;
                    }
                    //change status from rejected
                    else if (marketplaceProject.Status == ProjectStatus.Rejected && rejectedChangelist.Contains(status))
                    {
                        marketplaceProject.Status = status;
                        isChanged = true;
                    }
                    //change status from processing
                    else if (marketplaceProject.Status == ProjectStatus.Processing && processingChangelist.Contains(status))
                    {
                        marketplaceProject.Status = status;
                        isChanged = true;
                    }

                    if (isChanged)
                    {
                        _unitOfWork.MarketplaceRepository.Update(marketplaceProject);
                        await _unitOfWork.CommitAsync();

                        var response = _mapper.Map<MarketplaceProjectInfoResponse>(marketplaceProject);

                        return ResultDTO<MarketplaceProjectInfoResponse>.Success(response);
                    }
                    else throw new ExceptionError(
                        (int)HttpStatusCode.BadRequest,
                        $"Marketplace Project with status {marketplaceProject.Status} cannot be changed to {status}.");
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Marketplace Project Not Found.");
                }
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

        //validation
        private List<string> ValidateCommonFields(dynamic request)
        {
            try
            {
                List<string> errorMessages = new List<string>();

                if (string.IsNullOrEmpty(request.Name))
                {
                    errorMessages.Add("Name is required.");
                }

                if (string.IsNullOrEmpty(request.Description))
                {
                    errorMessages.Add("Description is required.");
                }

                if (string.IsNullOrEmpty(request.Introduction))
                {
                    errorMessages.Add("Introduction is required.");
                }

                if (request.Price <= 0)
                {
                    errorMessages.Add("Invalid price.");
                }

                return errorMessages;
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

        private List<string> ValidateMarketplaceProject(MarketplaceProjectAddRequest request)
        {
            var errorMessages = ValidateCommonFields(request);
            if (request.MarketplaceFiles.Count <= 0)
            {
                errorMessages.Add("Missing file(s).");
            }

            return errorMessages;
        }

        private List<string> ValidateMarketplaceProject(MarketplaceProjectUpdateRequest request)
        {
            return ValidateCommonFields(request);
        }

        //add files
        private List<MarketplaceFile> AddFiles(List<MarketplaceFileRequest> marketplaceFiles, Guid id)
        {
            List<MarketplaceFile> files = new List<MarketplaceFile>();

            foreach (MarketplaceFileRequest file in marketplaceFiles)
            {
                if (file.URL.Length > 0)
                {
                    var result = _azureService.UploadUrlSingleFiles(file.URL);

                    if (result == null)
                    {
                        throw new ExceptionError((int)HttpStatusCode.BadRequest, "Fail to upload file");
                    }

                    MarketplaceFile media = new MarketplaceFile
                    {
                        Name = file.Name,
                        URL = result.Result,
                        FileType = file.FileType,
                        CreatedDate = DateTime.Now,
                        MarketplaceProjectId = id
                    };

                    files.Add(media);
                }
            }

            return files;
        }

        //get non-deleted files
        private List<MarketplaceFile> GetNonDeletedFiles(List<MarketplaceFile> marketplaceFiles)
        {
            List<MarketplaceFile> files = new List<MarketplaceFile>();

            foreach (MarketplaceFile file in marketplaceFiles)
            {
                if (file.IsDeleted == false)
                {
                    files.Add(file);
                }
            }

            return files;
        }

        //get marketplace project wallet
        private async Task<Wallet?> GetMarketplaceProjectWallet(Guid marketplaceProjectId)
        {
            return await _unitOfWork.WalletRepository
                .GetQueryable()
                .Include(w => w.MarketplaceProject)
                .Where(w => w.MarketplaceProject.Id == marketplaceProjectId)
                .FirstOrDefaultAsync();
        }

        //get bank account by wallet id
        private async Task<BankAccount?> GetBankAccountById(Guid id)
        {
            return await _unitOfWork.BankAccountRepository
                .GetQueryable()
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<ResultDTO<List<MarketplaceProjectInfoResponse>>> GetTop3MostFundedOngoingMarketplaceProject()
        {
            try
            {
                var projects = await _unitOfWork.MarketplaceRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(p => p.MarketplaceFiles)
                    .Include(p => p.DigitalKeys)
                    .Include(p => p.FundingProject.Categories)
                    .Include(p => p.FundingProject)
                        .ThenInclude(p => p.User)
                    .Where(mp => mp.Status == ProjectStatus.Processing)
                    .OrderByDescending(p => p.DigitalKeys.Count())
                    .Take(3)
                    .ToListAsync();

                if (!projects.Any())
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "No Marketplace Project Found");
                }

                List<MarketplaceProjectInfoResponse> result = _mapper.Map<List<MarketplaceProjectInfoResponse>>(projects);

                return ResultDTO<List<MarketplaceProjectInfoResponse>>.Success(result, "Marketplace Project Found!");
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

        public async Task<ResultDTO<decimal>> CountPlatformProjects()
        {
            try
            {
                var marketplaceProjects = await _unitOfWork.MarketplaceRepository.GetQueryable().CountAsync();
                var fundingProjects = await _unitOfWork.FundingProjectRepository.GetQueryable().CountAsync();

                return ResultDTO<decimal>.Success(marketplaceProjects + fundingProjects, "Found total project!");
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
