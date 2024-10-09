using AutoMapper;
using Fun_Funding.Application.IEmailServices;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ReportDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Service
{
    public class ReportService : IReportService
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public ReportService(IUserService userService, IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<ResultDTO<ViolientReport>> CreateReportRequest(ReportRequest request)
        {
            var user = _userService.GetUserInfo().Result;
            User exitUser = _mapper.Map<User>(user._data);
            if (exitUser == null)
            {
                return ResultDTO<ViolientReport>.Fail("user must be authenticate");
            }
            if (request is null)
            {
                return ResultDTO<ViolientReport>.Fail("field can not be null");
            }
            var exitedProject = await _unitOfWork.FundingProjectRepository.GetByIdAsync(request.ProjectId);
            if (exitedProject is null)
            {
                return ResultDTO<ViolientReport>.Fail("can not found project");
            }
            try
            {
                ViolientReport report = new ViolientReport
                {
                    Id = Guid.NewGuid(),
                    FileUrls = request.FileUrls,
                    ProjectId = request.ProjectId,
                    ReporterId = exitUser.Id,
                    Content = request.Content,
                    IsHandle = false,
                    Date = DateTime.Now,
                };
                _unitOfWork.ReportRepository.Create(report);
                return ResultDTO<ViolientReport>.Success(report, "Successfull to create report");

            }
            catch (Exception ex)
            {
                return ResultDTO<ViolientReport>.Fail("something wrong!");
            }
        }

        public async Task<ResultDTO<PaginatedResponse<ViolientReport>>> GetAllReport(ListRequest request)
        {
            try
            {
                var list = _unitOfWork.ReportRepository.GetAllPaged(request);
                return ResultDTO<PaginatedResponse<ViolientReport>>.Success(list, "Successfull querry");
            }
            catch (Exception ex)
            {
                return ResultDTO<PaginatedResponse<ViolientReport>>.Fail("something wrong!");
            }
        }

        public async Task<ResultDTO<ViolientReport>> UpdateHandleReport(HandleRequest request)
        {
            var user = _userService.GetUserInfo().Result;
            User exitUser = _mapper.Map<User>(user._data);
            if (exitUser == null)
                return ResultDTO<ViolientReport>.Fail("user not authenticate");
            if (request == null) 
                return ResultDTO<ViolientReport>.Fail("request null");
            var exitedReport = _unitOfWork.ReportRepository.Get(x=>x.Id==request.ReportId);
            if (exitedReport == null)
                return ResultDTO<ViolientReport>.Fail("reportId null");
            var project = await _unitOfWork.FundingProjectRepository.GetByIdAsync(exitedReport.ProjectId);
            if (project == null)
               return ResultDTO<ViolientReport>.Fail("project null");
            var owner = await _unitOfWork.UserRepository.GetByIdAsync(project.UserId);
            if (owner == null)
                return ResultDTO<ViolientReport>.Fail("owner null");
            try
            {
                var reporter = await _unitOfWork.UserRepository.GetByIdAsync(exitedReport.ReporterId);
                var update = Builders<ViolientReport>.Update.Set(x=>x.IsHandle, true);
                _unitOfWork.ReportRepository.Update(x=>x.Id == request.ReportId, update);
                var response = _unitOfWork.ReportRepository.Get(x => x.Id == request.ReportId);

                //email
                await _emailService.SendReportAsync(owner.Email,project.Name,owner.FullName,exitedReport.Date, reporter.FullName,exitedReport.Content);
                return ResultDTO<ViolientReport>.Success(response, "Successfull updated");
            }
            catch (Exception ex)
            {
                return ResultDTO<ViolientReport>.Fail("something wrong!");
            }
        }

       
    }
}
