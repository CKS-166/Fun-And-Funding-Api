using Fun_Funding.Application.IService;
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
    public class BackgroundProcessService : IBackgroundProcessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private DateTime present = DateTime.Now;

        public BackgroundProcessService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                    if (project.Status == ProjectStatus.Processing && project.EndDate <= present && project.Balance < project.Target)
                    {
                        project.Status = ProjectStatus.Failed;
                        statusChanged = true;
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
                    // If the project has reached its funding goal
                    else if (project.Balance >= project.Target && project.Status == ProjectStatus.Processing)
                    {
                        project.Status = ProjectStatus.FundedSuccessful;
                        statusChanged = true;
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
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }


        
    }
}
