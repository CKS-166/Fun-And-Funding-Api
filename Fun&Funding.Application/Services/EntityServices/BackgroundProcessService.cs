using Fun_Funding.Application.Interfaces.IExternalServices;
using Fun_Funding.Application.IService;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class BackgroundProcessService : IBackgroundProcessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private DateTime present = DateTime.Now;
        private IEmailService _emailService;
        public BackgroundProcessService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }
        public async Task UpdateFundingStatus()
        {
            try
            {
                // Rewrite the query to avoid unsupported operations.
                List<FundingProject> projects = _unitOfWork.FundingProjectRepository
                    .GetQueryable()
                    .Where(p => (p.Status == ProjectStatus.Approved ||
                        p.Status == ProjectStatus.Pending ||
                        p.Status == ProjectStatus.Processing) && p.IsDeleted == false)
                    .ToList();

                foreach (var project in projects)
                {
                    bool statusChanged = false;
                    // If project stil present and end date has already pass
                    if (project.Status == ProjectStatus.Processing && project.EndDate <= present)
                    {
                        if (project.Balance < project.Target)
                        {
                            project.Status = ProjectStatus.Failed;
                            statusChanged = true;
                        }else if (project.Balance >= project.Target)
                        {
                            project.Status = ProjectStatus.FundedSuccessful;
                        }
                       
                    }
                    // If admin has already approved project and start date reach today's date
                    else if (project.Status == ProjectStatus.Approved)
                    {
                        if (project.StartDate >= present)
                        {
                            project.Status = ProjectStatus.Processing;
                            statusChanged = true;
                        }
                    }
                    
                    else if (project.Status == ProjectStatus.Pending && project.StartDate <= present)
                    {
                        project.Status = ProjectStatus.Rejected;
                        statusChanged = true;
                    }
                    if (statusChanged)
                    {
                        _unitOfWork.FundingProjectRepository.Update(project);
                    }
                }

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateProjectMilestoneStatus()
        {
            try
            {
                var projectMilestones = _unitOfWork.ProjectMilestoneRepository
                    .GetQueryable()
                    .Include(pm => pm.Milestone)
                    .Where(p => p.Status == ProjectMilestoneStatus.Pending
                    || p.Status == ProjectMilestoneStatus.Processing
                    || p.Status == ProjectMilestoneStatus.Warning).ToList();
                foreach (var projectMilestone in projectMilestones)
                {
                    bool statusChanged = false;
                    User owner = GetUserByProjectMilestoneId(projectMilestone.FundingProjectId);
                    FundingProject project = GetProject(projectMilestone.FundingProjectId);
                    if (projectMilestone.Status == ProjectMilestoneStatus.Processing)
                    {
                        if ((projectMilestone.EndDate.Date - present.Date).TotalDays == 7)
                        {
                            await _emailService.SendMilestoneAsync(owner.Email, project.Name, projectMilestone.Milestone.MilestoneName, owner.FullName, null, 7, present, EmailType.MilestoneReminder);
                        }
                        if ((projectMilestone.EndDate.Date - present.Date).TotalDays <= 0)
                        {
                            projectMilestone.Status = ProjectMilestoneStatus.Submitted;
                            await _emailService.SendMilestoneAsync(owner.Email, project.Name, projectMilestone.Milestone.MilestoneName, owner.FullName, "Submitted for review", null, present, EmailType.MilestoneExpired);
                            statusChanged = true;
                        }
                       

                    }
                    else if (projectMilestone.Status == ProjectMilestoneStatus.Warning)
                    {
                        if ((projectMilestone.EndDate.Date - present.Date).TotalDays == 7)
                        {
                            await _emailService.SendMilestoneAsync(owner.Email, project.Name, projectMilestone.Milestone.MilestoneName, owner.FullName, null, 7, present, EmailType.MilestoneReminder);
                        }else if ((projectMilestone.EndDate.Date - present).TotalDays <= 0)
                        {
                            projectMilestone.Status = ProjectMilestoneStatus.Resubmitted;
                            await _emailService.SendMilestoneAsync(owner.Email, project.Name, projectMilestone.Milestone.MilestoneName, owner.FullName, "Failed", null, present, EmailType.MilestoneExpired);
                            statusChanged = true;
                        }
                    }

                    if (statusChanged)
                    {
                        _unitOfWork.ProjectMilestoneRepository.Update(projectMilestone);
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public User GetUserByProjectMilestoneId(Guid projectId)
        {
            FundingProject project = _unitOfWork.FundingProjectRepository.GetQueryable()
                .Include(p => p.User).FirstOrDefault(p => p.Id == projectId);   
            if (project == null)
            {
                return null;
            }
            User owner = _unitOfWork.UserRepository.GetById(project.UserId);
            if(owner == null)
            {
                return null;
            }
            return owner;
        }

        public FundingProject GetProject(Guid projectId)
        {
            FundingProject project = _unitOfWork.FundingProjectRepository.GetQueryable()
               .Include(p => p.User).FirstOrDefault(p => p.Id == projectId);
            if (project == null)
            {
                return null;
            }
            return project;
        }
    }
}
