using AutoMapper;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.IStorageService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.FundingFileDTO;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
using Fun_Funding.Application.ViewModel.PackageDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Fun_Funding.Application.Service
{
    public class FundingProjectManagementService : IFundingProjectService
    {
        public IUnitOfWork _unitOfWork;
        public IMapper _mapper;
        public IAzureService _azureService;

        public FundingProjectManagementService(IUnitOfWork unitOfWork, IMapper mapper, IAzureService azureService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _azureService = azureService;
        }
        public async Task<ResultDTO<string>> CreateFundingProject(FundingProjectAddRequest projectRequest)
        {
            try
            {
                //map project
                FundingProject project = _mapper.Map<FundingProject>(projectRequest);
                //check owner
                User owner = _unitOfWork.UserRepository.GetQueryable().FirstOrDefault(u => u.Email == projectRequest.Email);
                if (owner == null)
                {
                    return ResultDTO<string>.Fail("Owner not found", 404);
                }
                project.User = owner;
                //validate package amount
                foreach (PackageAddRequest pack in projectRequest.Packages)
                {
                    if (pack.RequiredAmount < 5000)
                    {
                        return ResultDTO<string>.Fail("Price for package must be at least 5000");
                    }
                    if (pack.RewardItems.Count < 1)
                    {
                        return ResultDTO<string>.Fail("Each package must have at least 1 item");
                    }
                    if (pack.LimitQuantity < 1)
                    {
                        return ResultDTO<string>.Fail("Each package must limit at least 1 quantity");
                    }
                }
                //validate bank
                if (projectRequest.BankAccount is null)
                {
                    return ResultDTO<string>.Fail("Project must config its bank account for payment");
                }
                //validate startDate endDate info
                if (project.EndDate <= project.StartDate)
                {
                    return ResultDTO<string>.Fail("End date must be greater that start date");
                }
                if ((project.EndDate - project.StartDate).TotalDays < 60)
                {
                    return ResultDTO<string>.Fail("Funding campaign length must be at least 60 days");
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
                            return ResultDTO<string>.Fail("Fail to upload file");
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
                        if (rewardRequest?.ImageFile != null && rewardRequest.ImageFile is IFormFile)
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
                return ResultDTO<string>.Success("Add Sucessfully");
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
                FundingProject project = _unitOfWork.FundingProjectRepository.GetQueryable()
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

        public async Task<ResultDTO<string>> UpdateFundingProject(FundingProjectUpdateRequest projectRequest)
        {
            try
            {
                FundingProject existedProject = _unitOfWork.FundingProjectRepository.GetById(projectRequest.Id);
                if (existedProject == null)
                {
                    return ResultDTO<string>.Fail("Project not found", 404);
                }
                //update regulations for funding goals and end date

                //validate bank and package amount
                foreach (PackageUpdateRequest pack in projectRequest.Packages)
                {
                    if (pack.RequiredAmount < 5000)
                    {
                        return ResultDTO<string>.Fail("Price for package must be at least 5000");
                    }
                    if (pack.RewardItems.Count < 1)
                    {
                        return ResultDTO<string>.Fail("Each package must have at least 1 item");
                    }
                    if (pack.LimitQuantity < 1)
                    {
                        return ResultDTO<string>.Fail("Each package must limit at least 1 quantity");
                    }
                }
                if (projectRequest.BankAccount is null)
                {
                    return ResultDTO<string>.Fail("Project must config its bank account for payment");
                }

                _mapper.Map(projectRequest, existedProject);
                existedProject.CreatedDate = DateTime.Now;
                existedProject.Status = ProjectStatus.Pending;
                //update files 

                foreach (FundingFileUpdateRequest req in projectRequest.FundingFiles)
                {
                    if (req.URL.Length > 0 && req.URL is not null)
                    {
                        var res = _azureService.UploadUrlSingleFiles(req.URL);
                        FundingFile file = _unitOfWork.SourceFileRepository.GetQueryable().FirstOrDefault(f => f.Id == req.Id);
                        file.Name = req.Name;
                        file.URL = res.Result;
                    }

                }
                //add iamge into item 
                foreach (var package in existedProject.Packages)
                {
                    foreach (var rewardItem in package.RewardItems)
                    {
                        // Find the corresponding reward item in the request to get its ImageFile
                        var rewardRequest = projectRequest.Packages
                            .FirstOrDefault(p => p.Name == package.Name)?
                            .RewardItems.FirstOrDefault(r => r.Name == rewardItem.Name);

                        // If ImageFile is present, upload it and set the ImageUrl
                        if (rewardRequest?.ImageFile != null && rewardRequest.ImageFile is IFormFile)
                        {
                            var uploadResult = _azureService.UploadUrlSingleFiles(rewardRequest.ImageFile);
                            rewardItem.ImageUrl = uploadResult.Result; // Append the uploaded URL to the mapped reward item
                        }
                    }
                }

                return ResultDTO<string>.Success("Ok");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
