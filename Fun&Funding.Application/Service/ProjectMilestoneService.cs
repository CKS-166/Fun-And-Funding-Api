using AutoMapper;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ProjectMilestoneBackerDTO;
using Fun_Funding.Application.ViewModel.ProjectMilestoneDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Service
{
    public class ProjectMilestoneService : IProjectMilestoneService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private int maxExpireDay = 30;
        private int maxMilestoneExtend = 10;
        public ProjectMilestoneService(IUnitOfWork unitOfWork, IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultDTO<ProjectMilestoneResponse>> CreateProjectMilestoneRequest(ProjectMilestoneRequest request)
        {
            try
            {
                FundingProject project = _unitOfWork.FundingProjectRepository
                    .GetQueryable().Include(p => p.ProjectMilestones)
                    .ThenInclude(pm => pm.Milestone).FirstOrDefault(p => p.Id == request.FundingProjectId);
                if (project == null) {
                    return ResultDTO<ProjectMilestoneResponse>.Fail("Project not found", 404);
                }
                //case project not funded successfully
                if (project.Status != ProjectStatus.FundedSuccessful) {
                    return ResultDTO<ProjectMilestoneResponse>.Fail("Project is not funded successfully", 500);
                }
                

                Milestone requestMilestone = _unitOfWork.MilestoneRepository
                    .GetQueryable().Include(m => m.Requirements)
                    .FirstOrDefault(m => m.Id == request.MilestoneId);

                // Check if this milestone has already been added to the project
                bool milestoneExists = project.ProjectMilestones
                    .Any(pm => pm.Milestone.Id == requestMilestone.Id);

                if (milestoneExists)
                {
                    return ResultDTO<ProjectMilestoneResponse>.Fail("This milestone has already been added to the project.", 400);
                }
                //case request first
                if (((request.CreatedDate - project.EndDate).TotalDays > maxExpireDay) && requestMilestone.MilestoneOrder == 1)
                {
                    return ResultDTO<ProjectMilestoneResponse>.Fail("The milestone must begin within 30 days after the project's funding period ends.", 500);
                }
                var checkValidateMilstone = CanCreateProjectMilestone(project, requestMilestone.MilestoneOrder);
                if (checkValidateMilstone != null)
                {
                    return ResultDTO<ProjectMilestoneResponse>.Fail(checkValidateMilstone, 500);
                }
                ProjectMilestone projectMilestone = new ProjectMilestone
                {
                    EndDate = request.CreatedDate.AddDays(requestMilestone.Duration),
                    Status = ProjectMilestoneStatus.Pending,
                    MilestoneId = request.MilestoneId,
                    FundingProjectId = project.Id,
                    CreatedDate = request.CreatedDate,
                    IsDeleted = false
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
                    .GetQueryable()
                    .Include(pm => pm.Milestone)
                    .ThenInclude(pmr => pmr.Requirements)
                    .FirstOrDefault(pm => pm.Id == id);
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

        public string CanCreateProjectMilestone(FundingProject project, int requestedMilestoneOrder)
        {
            // Get all the project milestones ordered by MilestoneOrder
            var projectMilestones = project.ProjectMilestones
                .OrderBy(pm => pm.Milestone.MilestoneOrder)
                .ToList();
            if (projectMilestones != null || projectMilestones.Count != 0) {
                // Check if the requested milestone order is valid
                if (requestedMilestoneOrder > projectMilestones.Count + 1)
                    return "Requested milestone order is greater than the next available milestone"; // Requested milestone order is greater than the next available milestone

                // Check the status of the previous milestones
                for (int i = 0; i < requestedMilestoneOrder - 1; i++)
                {
                    var previousMilestone = projectMilestones[i];
                    if (previousMilestone.Status != ProjectMilestoneStatus.Completed)
                        return "The previous milestones are not completed";
                }
                if (requestedMilestoneOrder > 1)
                {
                    if ((projectMilestones[requestedMilestoneOrder].CreatedDate - projectMilestones[requestedMilestoneOrder - 1].EndDate).TotalDays > maxMilestoneExtend)
                    {
                        return "Requested days betweeen each milestone must be within 10 days";
                    }
                }
                
            }
            return null;
        }

        public async Task<ResultDTO<List<ProjectMilestoneResponse>>> GetAllProjectMilestone()
        {
            try
            {
                var pmList = await _unitOfWork.ProjectMilestoneRepository
                    .GetQueryable()
                    .Include(pmb => pmb.Milestone)
                        .ThenInclude(pm => pm.Requirements)
                    .Include(pmb => pmb.FundingProject)
                    .ToListAsync();

                var responseList = new List<ProjectMilestoneResponse>();

                foreach (var item in pmList)
                {
                    responseList.Add(_mapper.Map<ProjectMilestoneResponse>(item));
                }

                return ResultDTO<List<ProjectMilestoneResponse>>.Success(responseList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResultDTO<string>> UpdateProjectMilestoneStatus(ProjectMilestoneStatusUpdateRequest request)
        {
            try
            {
                var projectMilestone = await _unitOfWork.ProjectMilestoneRepository.GetAsync(pm => pm.Id == request.ProjectMilestoneId);
                if (projectMilestone == null) return ResultDTO<string>.Fail("The requested project milestone is not found!");

                // check project milestone current status
                // ...

                // check project milestone incoming status
                // ...

                projectMilestone.Status = request.Status;
                _unitOfWork.ProjectMilestoneRepository.Update(projectMilestone);

                await _unitOfWork.CommitAsync();

                return ResultDTO<string>.Success("Update successfully!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
