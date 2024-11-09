using AutoMapper;
using Fun_Funding.Application.Interfaces.IExternalServices;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ReportDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class ReportService : IReportService
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IAzureService _azureService;

        public ReportService(IUserService userService, IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IAzureService azureService)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _azureService = azureService;
        }

        public async Task<ResultDTO<ViolentReport>> CreateReportRequest(ReportRequest request)
        {
            var user = await _userService.GetUserInfo();
            User exitUser = _mapper.Map<User>(user._data);

            if (exitUser == null)
            {
                return ResultDTO<ViolentReport>.Fail("user must be authenticate");
            }
            if (request is null)
            {
                return ResultDTO<ViolentReport>.Fail("field can not be null");
            }
            var exitedProject = await _unitOfWork.FundingProjectRepository.GetByIdAsync(request.ProjectId);
            if (exitedProject is null)
            {
                return ResultDTO<ViolentReport>.Fail("can not found project");
            }
            try
            {
                // Upload files and get their URLs
                List<string> fileUrls = new List<string>();
                foreach (var file in request.FileUrls) // `Files` would be of type `IFormFile[]`
                {
                    // Upload each file and get its URL (pseudo-code)
                    string fileUrl = await _azureService.UploadUrlSingleFiles(file);
                    fileUrls.Add(fileUrl);
                }
                ViolentReport report = new ViolentReport
                {
                    Id = Guid.NewGuid(),
                    FileUrls = fileUrls,
                    ProjectId = request.ProjectId,
                    ReporterId = exitUser.Id,
                    Content = request.Content,
                    IsHandle = false,
                    Date = DateTime.Now,
                    FaultCauses = request.FaultCauses,
                };
                _unitOfWork.ReportRepository.Create(report);
                return ResultDTO<ViolentReport>.Success(report, "Successfull to create report");

            }
            catch (Exception ex)
            {
                return ResultDTO<ViolentReport>.Fail("something wrong!");
            }
        }

        public async Task<ResultDTO<PaginatedResponse<ViolentReport>>> GetAllReport(ListRequest request)
        {
            try
            {
                var list = _unitOfWork.ReportRepository.GetAllPaged(request);
                return ResultDTO<PaginatedResponse<ViolentReport>>.Success(list, "Successfull querry");
            }
            catch (Exception ex)
            {
                return ResultDTO<PaginatedResponse<ViolentReport>>.Fail("something wrong!");
            }
        }

        public async Task<ResultDTO<ViolentReport>> UpdateHandleReport(HandleRequest request)
        {
            var user = _userService.GetUserInfo().Result;
            User exitUser = _mapper.Map<User>(user._data);
            if (exitUser == null)
                return ResultDTO<ViolentReport>.Fail("user not authenticate");
            if (request == null)
                return ResultDTO<ViolentReport>.Fail("request null");
            var exitedReport = _unitOfWork.ReportRepository.Get(x => x.Id == request.ReportId);
            if (exitedReport == null)
                return ResultDTO<ViolentReport>.Fail("reportId null");
            var project = await _unitOfWork.FundingProjectRepository.GetByIdAsync(exitedReport.ProjectId);
            if (project == null)
                return ResultDTO<ViolentReport>.Fail("project null");
            var owner = await _unitOfWork.UserRepository.GetByIdAsync(project.UserId);
            if (owner == null)
                return ResultDTO<ViolentReport>.Fail("owner null");
            try
            {
                var reporter = await _unitOfWork.UserRepository.GetByIdAsync(exitedReport.ReporterId);
                var update = Builders<ViolentReport>.Update.Set(x => x.IsHandle, true);
                _unitOfWork.ReportRepository.Update(x => x.Id == request.ReportId, update);
                var response = _unitOfWork.ReportRepository.Get(x => x.Id == request.ReportId);

                //email
                await _emailService.SendReportAsync(owner.Email, project.Name, owner.FullName, exitedReport.Date, reporter.FullName, exitedReport.Content);
                return ResultDTO<ViolentReport>.Success(response, "Successfull updated");
            }
            catch (Exception ex)
            {
                return ResultDTO<ViolentReport>.Fail("something wrong!");
            }
        }


    }
}
