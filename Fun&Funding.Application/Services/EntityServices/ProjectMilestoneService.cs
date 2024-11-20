using AutoMapper;
using Fun_Funding.Application.ExceptionHandler;
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
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class ProjectMilestoneService : IProjectMilestoneService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private int maxExpireDay = 30;
        private int maxMilestoneExtend = 10;
        private ITransactionService _transactionService;
        public ProjectMilestoneService(IUnitOfWork unitOfWork, IMapper mapper, ITransactionService transactionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _transactionService = transactionService;
        }

        public async Task<ResultDTO<ProjectMilestoneResponse>> CreateProjectMilestoneRequest(ProjectMilestoneRequest request)
        {
            try
            {
                FundingProject project = _unitOfWork.FundingProjectRepository
                    .GetQueryable().Include(p => p.ProjectMilestones)
                    .ThenInclude(pm => pm.Milestone).FirstOrDefault(p => p.Id == request.FundingProjectId);
                if (project == null)
                {
                    return ResultDTO<ProjectMilestoneResponse>.Fail("Project not found", 404);
                }
                //case project not funded successfully
                if (project.Status != ProjectStatus.FundedSuccessful)
                {
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
                if (requestMilestone.MilestoneOrder == 1)
                {
                    if (request.CreatedDate >= project.EndDate)
                    {
                        if ((request.CreatedDate - project.EndDate).TotalDays > maxExpireDay)
                        {
                            return ResultDTO<ProjectMilestoneResponse>.Fail("The milestone must begin within 30 days after the project's funding period ends.", 500);
                        }
                    }
                    else
                    {
                        throw new ExceptionError((int)HttpStatusCode.BadRequest, "First milestone requested date must be after the date project funded successfully");
                    }

                }


                var checkValidateMilstone = CanCreateProjectMilestone(project, requestMilestone.MilestoneOrder, requestMilestone.CreatedDate);
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
                    IsDeleted = false,

                };

                await _unitOfWork.ProjectMilestoneRepository.AddAsync(projectMilestone);
                _unitOfWork.Commit();
                return GetProjectMilestoneRequest(projectMilestone.Id).Result;

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

        public async Task<ResultDTO<ProjectMilestoneResponse>> GetProjectMilestoneRequest(Guid id)
        {
            try
            {
                ProjectMilestone projectMilestone = _unitOfWork.ProjectMilestoneRepository
                    .GetQueryable()
                    .Include(pm => pm.Milestone)
                    .ThenInclude(pmr => pmr.Requirements)
                    .Include(pm => pm.ProjectMilestoneRequirements) // Include ProjectMilestoneRequirements collection
                    .ThenInclude(pmr => pmr.Requirement) // Include Requirement within ProjectMilestoneRequirements
                    .Include(pm => pm.ProjectMilestoneRequirements) // Include ProjectMilestoneRequirements collection again
                    .ThenInclude(pmr => pmr.RequirementFiles)
                    .FirstOrDefault(pm => pm.Id == id);
                if (projectMilestone == null)
                {
                    return ResultDTO<ProjectMilestoneResponse>.Fail("Not found", 404);
                }
                ProjectMilestoneResponse result = _mapper.Map<ProjectMilestoneResponse>(projectMilestone);
                return ResultDTO<ProjectMilestoneResponse>.Success(result);
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

        public string CanCreateProjectMilestone(FundingProject project, int requestedMilestoneOrder, DateTime createdDate)
        {
            // Get all the project milestones ordered by MilestoneOrder
            var projectMilestones = project.ProjectMilestones
                .OrderBy(pm => pm.Milestone.MilestoneOrder)
                .ToList();
            if (projectMilestones != null || projectMilestones.Count != 0)
            {
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
                    var previousMilestone = projectMilestones[requestedMilestoneOrder - 2];
                    if ((createdDate - previousMilestone.EndDate).TotalDays > maxMilestoneExtend)
                    {
                        return $"Requested days between each milestone must be within {maxMilestoneExtend} days";
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
                if (ex is ExceptionError exceptionError)
                {
                    throw exceptionError;
                }

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
                var pendingStatusList = new List<ProjectMilestoneStatus>() { ProjectMilestoneStatus.Processing };
                var processingStatusList = new List<ProjectMilestoneStatus>() { ProjectMilestoneStatus.Submitted };
                var approvedStatusList = new List<ProjectMilestoneStatus>()
                {
                    ProjectMilestoneStatus.Completed,
                    ProjectMilestoneStatus.Warning
                };
                var warnStatusList = new List<ProjectMilestoneStatus>()
                {
                    ProjectMilestoneStatus.Completed,
                    ProjectMilestoneStatus.Failed
                };
                // check project milestone incoming status
                // ...
                bool statusChanged = false;
                if (projectMilestone.Status == ProjectMilestoneStatus.Pending && pendingStatusList.Contains(request.Status))
                {
                    projectMilestone.Status = request.Status;
                    statusChanged = true;
                }
                else if (projectMilestone.Status == ProjectMilestoneStatus.Processing && processingStatusList.Contains(request.Status))
                {
                    projectMilestone.Status = request.Status;
                    statusChanged = true;
                    if (request.Status.Equals(ProjectMilestoneStatus.Warning))
                    {
                        if (request.NewEndDate == null)
                        {
                            return ResultDTO<string>.Fail("New end date is required for warning a project milestone!");
                        }
                        else
                        {
                            projectMilestone.EndDate = request.NewEndDate.Value;
                        }
                    }
                }
                else if (projectMilestone.Status == ProjectMilestoneStatus.Submitted && approvedStatusList.Contains(request.Status))
                {
                    projectMilestone.Status = request.Status;
                    statusChanged = true;
                }
                else if (projectMilestone.Status == ProjectMilestoneStatus.Warning && warnStatusList.Contains(request.Status))
                {
                    projectMilestone.Status = request.Status;
                    statusChanged = true;
                }
                if (statusChanged)
                {
                    _unitOfWork.ProjectMilestoneRepository.Update(projectMilestone);

                    await _unitOfWork.CommitAsync();
                }
                else throw new ExceptionError(
                       (int)HttpStatusCode.BadRequest,
                       $"Milestone with status {projectMilestone.Status} cannot be changed to {request.Status}.");

                return ResultDTO<string>.Success($"Update successfully to {request.Status}!");
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

        public async Task<ResultDTO<PaginatedResponse<ProjectMilestoneResponse>>> GetProjectMilestones(
            ListRequest request,
            ProjectMilestoneStatus? status,
            Guid? fundingProjectId,
            Guid? milestoneId)
        {
            try
            {
                // Initialize the filter with a default condition that always evaluates to true.
                Expression<Func<ProjectMilestone, bool>> filter = u => true;


                // Apply status filter.
                if (status != null)
                {
                    filter = u => u.Status == status;
                }

                // Apply FundingProjectId and milestoneId filters.
                if (fundingProjectId != null && milestoneId != null)
                {
                    // Both parameters are provided, so combine them with &&.
                    filter = u => u.FundingProjectId == fundingProjectId && u.MilestoneId == milestoneId;
                }
                else if (fundingProjectId != null)
                {
                    // Only FundingProjectId is provided.
                    filter = u => u.FundingProjectId == fundingProjectId;
                }
                else if (milestoneId != null)
                {
                    // Only milestoneId is provided.
                    filter = u => u.MilestoneId == milestoneId;
                }

                // Apply date filters.
                if (request.From is DateTime fromDate)
                {
                    filter = u => u.CreatedDate >= fromDate;
                }

                if (request.To is DateTime toDate)
                {
                    filter = u => u.EndDate <= toDate;
                }

                // Define the orderBy expression.
                Expression<Func<ProjectMilestone, object>> orderBy = u => u.CreatedDate;
                if (!string.IsNullOrEmpty(request.OrderBy))
                {
                    switch (request.OrderBy.ToLower())
                    {
                        case "enddate":
                            orderBy = u => u.EndDate;
                            break;
                        case "status":
                            orderBy = u => u.Status;
                            break;
                        default:
                            break;
                    }
                }

                // Retrieve the paginated list of milestones.
                var list = await _unitOfWork.ProjectMilestoneRepository.GetAllAsync(
                    filter: filter,
                    orderBy: orderBy,
                    isAscending: request.IsAscending ?? true,
                    pageIndex: request.PageIndex ?? 1,
                    pageSize: request.PageSize ?? 10,
                    includeProperties: "Milestone,FundingProject,FundingProject.SourceFiles,FundingProject.User,FundingProject.Wallet,FundingProject.Wallet.BankAccount" +
                    ",ProjectMilestoneRequirements.RequirementFiles,ProjectMilestoneRequirements.Requirement"
                );

                    var totalItems = _unitOfWork.ProjectMilestoneRepository.GetAll(filter).Count();
                    var totalPages = (int)Math.Ceiling((double)totalItems / (request.PageSize ?? 10));

                    // Map to the response DTO.
                    var responseItems = _mapper.Map<IEnumerable<ProjectMilestoneResponse>>(list);

                    var response = new PaginatedResponse<ProjectMilestoneResponse>
                    {
                        PageSize = request.PageSize ?? 10,
                        PageIndex = request.PageIndex ?? 1,
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        Items = responseItems
                    };

                    return ResultDTO<PaginatedResponse<ProjectMilestoneResponse>>.Success(response);
                

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
