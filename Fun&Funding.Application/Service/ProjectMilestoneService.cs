using AutoMapper;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ProjectMilestoneDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Service
{
    public class ProjectMilestoneService : IProjectMilestoneService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private int maxExpireDay = 30;
        public ProjectMilestoneService(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDTO<ProjectMilestoneResponse>> CreateProjectMilestoneRequest(ProjectMilestoneRequest request)
        {
            try
            {
                FundingProject project = _unitOfWork.FundingProjectRepository.GetQueryable().FirstOrDefault(p => p.Id == request.FundingProjectId);
                if (project == null) {
                    return ResultDTO<ProjectMilestoneResponse>.Fail("Project not found", 404);
                }
                //case milestone 1
                if (project.Status != ProjectStatus.FundedSucessful) {
                    return ResultDTO<ProjectMilestoneResponse>.Fail("Project is not funded successfully", 500);
                }
                if ((request.CreatedDate - project.EndDate).TotalDays > maxExpireDay)
                {
                    return ResultDTO<ProjectMilestoneResponse>.Fail("The milestone must begin within 30 days after the project's funding period ends.", 500);
                }

                Milestone requestMilestone = _unitOfWork.MilestoneRepository
                    .GetQueryable().Include(m => m.Requirements)
                    .FirstOrDefault(m => m.Id == request.MilestoneId);

                List<ProjectMilestoneRequirement> milestoneRequirements = new List<ProjectMilestoneRequirement>();
                foreach (Requirement req in requestMilestone.Requirements) { 
                    ProjectMilestoneRequirement requirement = new ProjectMilestoneRequirement
                    {
                        RequirementStatus = RequirementStatus.Processing,
                        RequirementId = req.Id,
                        CreatedDate = request.CreatedDate,
                        IsDeleted = false,
                        Content = ""
                    };
                    milestoneRequirements.Add(requirement);
                }

                ProjectMilestone projectMilestone = new ProjectMilestone
                {
                    EndDate = request.CreatedDate.AddDays(requestMilestone.Duration),
                    Status = ProjectMilestoneStatus.Pending,
                    MilestoneId = request.MilestoneId,
                    FundingProjectId = project.Id,
                    CreatedDate = request.CreatedDate,
                    IsDeleted = false,
                    ProjectMilestoneRequirements = milestoneRequirements
                };

                await _unitOfWork.ProjectMilestoneRepository.AddAsync(projectMilestone);
                _unitOfWork.Commit();
                return GetProjectMilestoneRequest(projectMilestone.Id).Result;

                }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResultDTO<ProjectMilestoneResponse>> GetProjectMilestoneRequest(Guid id)
        {
            try
            {
                ProjectMilestone projectMilestone =  _unitOfWork.ProjectMilestoneRepository
                    .GetQueryable().Include(pm => pm.ProjectMilestoneRequirements).FirstOrDefault(pm => pm.Id == id);
                if (projectMilestone == null) {
                    return ResultDTO<ProjectMilestoneResponse>.Fail("Not found", 404);
                }
                ProjectMilestoneResponse result = _mapper.Map<ProjectMilestoneResponse>(projectMilestone);
                return ResultDTO<ProjectMilestoneResponse>.Success(result);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }
}
