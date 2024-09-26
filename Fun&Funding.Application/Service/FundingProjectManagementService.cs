using AutoMapper;
using Azure.Core;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.IStorageService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.CategoryDTO;
using Fun_Funding.Application.ViewModel.FundingFileDTO;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
using Fun_Funding.Application.ViewModel.PackageDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.ConstrainedExecution;

namespace Fun_Funding.Application.Service
{
    public class FundingProjectManagementService : IFundingProjectService
    {
        public IUnitOfWork _unitOfWork;
        public IMapper _mapper;
        public IAzureService _azureService;
        private int maxDays = 60;
        private int minDays = 1;
        public FundingProjectManagementService(IUnitOfWork unitOfWork, IMapper mapper, IAzureService azureService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _azureService = azureService;
        }
        public async Task<ResultDTO<FundingProjectResponse>> CreateFundingProject(FundingProjectAddRequest projectRequest)
        {
            try
            {
                //map project
                FundingProject project = _mapper.Map<FundingProject>(projectRequest);
                //check owner
                User owner = _unitOfWork.UserRepository.GetQueryable().FirstOrDefault(u => u.Email == projectRequest.Email);
                if (owner == null)
                {
                    return ResultDTO<FundingProjectResponse>.Fail("Owner not found", 404);
                }
                project.User = owner;
                //validate package amount
                foreach (PackageAddRequest pack in projectRequest.Packages)
                {
                    if (pack.RequiredAmount < 5000)
                    {
                        return ResultDTO<FundingProjectResponse>.Fail("Price for package must be at least 5000");
                    }
                    if (pack.RewardItems.Count < 1)
                    {
                        return ResultDTO<FundingProjectResponse>.Fail("Each package must have at least 1 item");
                    }
                    if (pack.LimitQuantity < 1)
                    {
                        return ResultDTO<FundingProjectResponse>.Fail("Each package must limit at least 1 quantity");
                    }
                }
                //validate bank
                if (projectRequest.BankAccount is null)
                {
                    return ResultDTO<FundingProjectResponse>.Fail("Project must config its bank account for payment");
                }
                //validate startDate endDate info
                if (project.StartDate < DateTime.Now)
                {
                    return ResultDTO<FundingProjectResponse>.Fail("Start date cannot be before today");
                }
                if (project.EndDate <= project.StartDate)
                {
                    return ResultDTO<FundingProjectResponse>.Fail("End date must be greater that start date");
                }
                if ((project.EndDate - project.StartDate).TotalDays <= minDays || (project.EndDate - project.StartDate).TotalDays >= maxDays)
                {
                    return ResultDTO<FundingProjectResponse>.Fail("Funding campaign length must be at least 1 day and maximum 60 days");
                }
                project.CreatedDate = DateTime.Now;
                project.Status = ProjectStatus.Pending;
                //free package
                Package freePack = new Package
                {
                    Name = "Non-package support",
                    Description = "We offer the \"Non-Package Support\" option, allowing you to contribute any amount you choose. This flexible choice doesn’t include rewards " +
                    "but greatly supports our project. Any amount given is greatly appreciated!",
                    PackageTypes = PackageType.Free,
                    CreatedDate = DateTime.Now,
                    RequiredAmount = 0
                };

                //add files 
                List<FundingFile> files = new List<FundingFile>();

                foreach (FundingFileRequest req in projectRequest.FundingFiles)
                {
                    if (req.URL.Length > 0)
                    {
                        var res = _azureService.UploadUrlSingleFiles(req.URL);
                        if (res == null)
                        {
                            return ResultDTO<FundingProjectResponse>.Fail("Fail to upload file");
                        }
                        FundingFile media = new FundingFile
                        {
                            Name = req.Name,
                            URL = res.Result,
                            Filetype = req.Filetype
                        };
                        files.Add(media);
                    }
                }
                //add iamge into item 
                foreach (var package in project.Packages)
                {
                    foreach (var rewardItem in package.RewardItems)
                    {
                        // Find the corresponding reward item in the request to get its ImageFile
                        var rewardRequest = projectRequest.Packages
                            .FirstOrDefault(p => p.Name == package.Name)?
                            .RewardItems.FirstOrDefault(r => r.Name == rewardItem.Name);

                        // If ImageFile is present, upload it and set the ImageUrl
                        if (rewardRequest?.ImageFile != null)
                        {
                            var uploadResult = _azureService.UploadUrlSingleFiles(rewardRequest.ImageFile);
                            rewardItem.ImageUrl = uploadResult.Result; // Append the uploaded URL to the mapped reward item
                        }
                    }
                    package.CreatedDate = DateTime.Now;
                    package.PackageTypes = PackageType.FixedPackage;
                }

                project.SourceFiles = files;
                project.Packages.Add(freePack);
                _unitOfWork.FundingProjectRepository.Add(project);
                _unitOfWork.Commit();
                return GetProjectById(project.Id).Result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResultDTO<FundingProjectResponse>> GetProjectById(Guid id)
        {
            try
            {
                FundingProject project =  _unitOfWork.FundingProjectRepository.GetQueryable()
                    .Include(p => p.Packages).ThenInclude(pack => pack.RewardItems)
                    .Include(p => p.SourceFiles)
                    .FirstOrDefault(p => p.Id == id);
                if (project is null)
                {
                    return ResultDTO<FundingProjectResponse>.Fail("Project not found", 404);
                }
                FundingProjectResponse result = _mapper.Map<FundingProjectResponse>(project);
                return ResultDTO<FundingProjectResponse>.Success(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResultDTO<FundingProjectResponse>> UpdateFundingProject(FundingProjectUpdateRequest projectRequest)
        {
            try
            {
                var existedProject = _unitOfWork.FundingProjectRepository.GetQueryable()
                .Include(p => p.SourceFiles)
                .Include(p => p.Packages).ThenInclude(pack => pack.RewardItems)
                .FirstOrDefault(o => o.Id == projectRequest.Id);
                // check status
                if (existedProject == null)
                {
                    return ResultDTO<FundingProjectResponse>.Fail("Project not found", 404);
                }
                //update regulations for funding goals and end date

                //validate bank and package amount
                foreach (PackageUpdateRequest pack in projectRequest.Packages)
                {
                    if (pack.RequiredAmount < 5000)
                    {
                        return ResultDTO<FundingProjectResponse>.Fail("Price for package must be at least 5000");
                    }
                    if (pack.RewardItems.Count < 1)
                    {
                        return ResultDTO<FundingProjectResponse>.Fail("Each package must have at least 1 item");
                    }
                    if (pack.LimitQuantity < 1)
                    {
                        return ResultDTO<FundingProjectResponse>.Fail("Each package must limit at least 1 quantity");
                    }
                }
                if (projectRequest.BankAccount is null)
                {
                    return ResultDTO<FundingProjectResponse>.Fail("Project must config its bank account for payment");
                }

                existedProject.Name = projectRequest.Name;
                existedProject.Description = projectRequest.Description;
                BankAccount bank = _mapper.Map<BankAccount>(projectRequest.BankAccount);
                existedProject.BankAccount = bank;
                existedProject.Introduction = projectRequest.Introduction;
                
                //update files 
                if (projectRequest.FundingFiles?.Count > 0)
                {
                    List<FundingFile> files = new List<FundingFile>();

                    foreach (FundingFileUpdateRequest req in projectRequest.FundingFiles)
                    {
                        if (req.URL.Length > 0)
                        {
                            var res = _azureService.UploadUrlSingleFiles(req.URL);
                            if (res == null)
                            {
                                return ResultDTO<FundingProjectResponse>.Fail("Fail to upload file");
                            }
                            FundingFile media = new FundingFile
                            {
                                Name = req.Name,
                                URL = res.Result,
                                Filetype = req.Filetype
                            };
                            files.Add(media);
                        }
                    }
                    // Add each file from 'files' list to the 'SourceFiles' ICollection
                    foreach (var file in files)
                    {
                        existedProject.SourceFiles.Add(file);
                    }
                }
                List<Package> packageList = new List<Package>();
                //add image into item 
                foreach (var packageRequest in projectRequest.Packages)
                {
                    var existedPack = existedProject.Packages.FirstOrDefault(p => p.Id == packageRequest.Id);
                    if (existedPack != null)
                    {
                        existedPack.Name = packageRequest.Name;
                        existedPack.RequiredAmount = packageRequest.RequiredAmount;
                        existedPack.LimitQuantity = packageRequest.LimitQuantity;
                        existedPack.PackageTypes = PackageType.FixedPackage;
                        //Handle change image of existing item
                        foreach (var rewardItemRequest in packageRequest.RewardItems)
                        {
                            var existedRewardItem = _unitOfWork.RewardItemRepository.GetQueryable().FirstOrDefault(r => r.Id == rewardItemRequest.Id);
                            if (existedRewardItem != null)
                            {
                                // Update existing reward item
                                existedRewardItem.Name = rewardItemRequest.Name;
                                existedRewardItem.Description = rewardItemRequest.Description;
                                existedRewardItem.Quantity = rewardItemRequest.Quantity;

                                // Handle image upload for reward item
                                if (rewardItemRequest.ImageFile != null && rewardItemRequest.ImageFile is IFormFile)
                                {
                                    var imageUploadResult = _azureService.UploadUrlSingleFiles(rewardItemRequest.ImageFile);
                                    existedRewardItem.ImageUrl = imageUploadResult.Result;
                                }
                            }
                            else
                            {
                                // Handle adding new reward items if necessary
                                RewardItem newRewardItem = new RewardItem
                                {
                                    Name = rewardItemRequest.Name,
                                    Description = rewardItemRequest.Description,
                                    // Handle image upload for new reward item
                                };
                                if (rewardItemRequest.ImageFile != null && rewardItemRequest.ImageFile is IFormFile)
                                {
                                    var imageUploadResult = _azureService.UploadUrlSingleFiles(rewardItemRequest.ImageFile);
                                    newRewardItem.ImageUrl = imageUploadResult.Result;
                                }
                                existedPack.RewardItems.Add(newRewardItem);
                            }
                        }
                    }
                    else
                    {
                        Package newPackage = new Package
                        {
                            Name = packageRequest.Name,
                            RequiredAmount = packageRequest.RequiredAmount,
                            LimitQuantity = packageRequest.LimitQuantity,
                            PackageTypes = PackageType.FixedPackage,
                            RewardItems = new List<RewardItem>()
                        };
                        // Add reward items to the new package
                        foreach (var rewardItemRequest in packageRequest.RewardItems)
                        {
                            RewardItem newRewardItem = new RewardItem
                            {
                                Name = rewardItemRequest.Name,
                                Description = rewardItemRequest.Description,
                                Quantity = rewardItemRequest.Quantity,
                            };

                            // Handle image upload for new reward item
                            if (rewardItemRequest.ImageFile != null && rewardItemRequest.ImageFile is IFormFile)
                            {
                                var imageUploadResult = _azureService.UploadUrlSingleFiles(rewardItemRequest.ImageFile);
                                newRewardItem.ImageUrl = imageUploadResult.Result;
                            }

                            newPackage.RewardItems.Add(newRewardItem);
                        }
                        // Add the new package to the project's packages
                        existedProject.Packages.Add(newPackage);
                    }
                }
                
                _unitOfWork.FundingProjectRepository.Update(existedProject);

                _unitOfWork.Commit();
                FundingProjectResponse result = _mapper.Map<FundingProjectResponse>(existedProject);
                return ResultDTO<FundingProjectResponse>.Success(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ResultDTO<PaginatedResponse<FundingProjectResponse>>> GetFundingProjects(ListRequest request, string? categoryName, ProjectStatus status, decimal? fromTarget, decimal? toTarget)
        {
            try
            {
                Expression<Func<FundingProject, bool>> filter = null;
                Expression<Func<FundingProject, object>> orderBy = u => u.CreatedDate;

                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    string searchLower = request.SearchValue.ToLower();
                    filter = u =>
                        (u.Name != null && u.Name.ToLower().Contains(searchLower));
                }
                if (!string.IsNullOrEmpty(request.OrderBy))
                {
                    switch (request.OrderBy.ToLower())
                    {
                        case "balance":
                            orderBy = u => u.Balance;
                            break;
                        case "target":
                            orderBy = u => u.Target;
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(categoryName))
                {
                    filter = c => c.Name.ToLower().Contains(categoryName);
                }
                if (request.From != null)
                {
                    filter = c => c.StartDate >= (DateTime)request.From;
                }
                if (request.To != null)
                {
                    filter = c => c.EndDate >= (DateTime)request.To;
                }
                if (status != null)
                {
                    filter = c => c.Status.Equals(status);
                }
                else
                {
                    filter = c => c.Status.Equals(ProjectStatus.Processing);
                }
                if (fromTarget != null)
                {
                    filter = c => c.Target >= fromTarget;
                }
                if (toTarget != null)
                {
                    filter = c => c.Target <= toTarget;
                }
                var list = await _unitOfWork.FundingProjectRepository.GetAllAsync(
                       filter: filter,
                       orderBy: orderBy,
                       isAscending: request.IsAscending.Value,
                       pageIndex: request.PageIndex,
                       pageSize: request.PageSize,
                       includeProperties: "Categories,Packages,SourceFiles,Packages.RewardItems");
                if (list != null && list.Count() > 0)
                {
                    var totalItems = _unitOfWork.FundingProjectRepository.GetAll(filter).Count();
                    var totalPages = (int)Math.Ceiling((double)totalItems / (int)request.PageSize);
                    IEnumerable<FundingProjectResponse> categories = _mapper.Map<IEnumerable<FundingProjectResponse>>(list);

                    PaginatedResponse<FundingProjectResponse> response = new PaginatedResponse<FundingProjectResponse>
                    {
                        PageSize = request.PageSize.Value,
                        PageIndex = request.PageIndex.Value,
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        Items = categories
                    };

                    return ResultDTO<PaginatedResponse<FundingProjectResponse>>.Success(response);
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Project Not Found.");
                }
            }
            catch (Exception ex)
            {
                 throw new ExceptionError((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ResultDTO<FundingProjectResponse>> UpdateFundingProjectStatus(Guid id, ProjectStatus status)
        {
            try
            {
                var project = await _unitOfWork.FundingProjectRepository.GetQueryable()
                    .Include(p => p.SourceFiles)
                    .Include(p => p.Packages)
                    .Include(p => p.User)
                    .Include(p => p.BankAccount)
                    .FirstOrDefaultAsync(p => p.Id == id);

                //pending status change list
                List<ProjectStatus> pendingChangelist = new List<ProjectStatus>()
                {
                    ProjectStatus.Approved,
                    ProjectStatus.Rejected,
                    ProjectStatus.Deleted
                };

                bool isChanged = false;

                if (project != null)
                {
                    //change status from pending
                    if (project.Status == ProjectStatus.Pending && pendingChangelist.Contains(status))
                    {
                        project.Status = status;
                        isChanged = true;
                    }
                    //other status
                    else if (false)
                    {

                    }

                    if (isChanged)
                    {
                        _unitOfWork.FundingProjectRepository.Update(project);
                        await _unitOfWork.CommitAsync();

                        var response = _mapper.Map<FundingProject, FundingProjectResponse>(project);

                        return ResultDTO<FundingProjectResponse>.Success(response);
                    }
                    else throw new ExceptionHandler.ExceptionError(
                        (int)HttpStatusCode.BadRequest,
                        $"Funding Project with status {project.Status} cannot be changed to {status}.");
                }
                else
                {
                    throw new ExceptionHandler.ExceptionError((int)HttpStatusCode.NotFound, "Funding Project Not Found.");
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
    }
}
