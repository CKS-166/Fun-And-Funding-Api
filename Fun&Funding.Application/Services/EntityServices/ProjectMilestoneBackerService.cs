using AutoMapper;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ProjectMilestoneBackerDTO;
using Fun_Funding.Application.ViewModel.ProjectMilestoneDTO;
using Fun_Funding.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class ProjectMilestoneBackerService : IProjectMilestoneBackerService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ProjectMilestoneBackerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResultDTO<ProjectMilestoneBackerResponse>> CreateNewProjectMilestoneBackerReview(ProjectMilestoneBackerRequest request)
        {
            try
            {
                var backer = await _unitOfWork.UserRepository.GetAsync(u => u.Id == request.BackerId);
                if (backer == null) return ResultDTO<ProjectMilestoneBackerResponse>.Fail("Backer not found!");

                var projectMilestone = await _unitOfWork.ProjectMilestoneRepository.GetAsync(m => m.Id == request.ProjectMilestoneId);
                if (projectMilestone == null) return ResultDTO<ProjectMilestoneBackerResponse>.Fail("Project milestone not found!");

                // check project milestone status
                if (projectMilestone.Status != Domain.Enum.ProjectMilestoneStatus.Processing || projectMilestone.Status != Domain.Enum.ProjectMilestoneStatus.Warning)
                    return ResultDTO<ProjectMilestoneBackerResponse>.Fail("This project milestone is currently not accepting reviews!");

                // check if backer donate
                var fundingProject = await _unitOfWork.FundingProjectRepository.GetAsync(p => p.ProjectMilestones.Any(p => p.Id == request.ProjectMilestoneId));
                if (fundingProject == null) return ResultDTO<ProjectMilestoneBackerResponse>.Fail("Funding project not found!");

                var isBacker = await _unitOfWork.PackageBackerRepository.GetQueryable()
                    .AnyAsync(pb => pb.Package.ProjectId == fundingProject.Id && pb.UserId == request.BackerId);


                // check if backer already review this projectmilestone
                var alreadyReview = await _unitOfWork.ProjectMilestoneBackerRepository.GetQueryable()
                    .AnyAsync(pmb => pmb.BackerId == request.BackerId);
                if (alreadyReview) return ResultDTO<ProjectMilestoneBackerResponse>.Fail("Backer already review this project milestone!");

                if (isBacker)
                {
                    var newReview = new ProjectMilestoneBacker
                    {
                        Star = request.Star,
                        Comment = request.Comment,
                        BackerId = request.BackerId,
                        Backer = backer,
                        ProjectMilestoneId = request.ProjectMilestoneId,
                        ProjectMilestone = projectMilestone,
                        CreatedDate = DateTime.Now,
                    };

                    _unitOfWork.ProjectMilestoneBackerRepository.Add(newReview);

                    await _unitOfWork.CommitAsync();

                    var response = _mapper.Map<ProjectMilestoneBackerResponse>(newReview);
                    return ResultDTO<ProjectMilestoneBackerResponse>.Success(response, "Add review successfully!");
                }

                return ResultDTO<ProjectMilestoneBackerResponse>.Fail("You must be a backer to review this milestone!");


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public async Task<ResultDTO<List<ProjectMilestoneBackerResponse>>> GetAllMilestoneReview(Guid projectMilestoneId)
        {
            try
            {
                var reviewList = await _unitOfWork.ProjectMilestoneBackerRepository
                    .GetQueryable()
                    //.Include(pmb => pmb.ProjectMilestone)
                    .Include(pmb => pmb.Backer)
                    .Where(pmb => pmb.ProjectMilestoneId == projectMilestoneId)
                    .ToListAsync();

                var responseList = new List<ProjectMilestoneBackerResponse>();

                foreach (var item in reviewList)
                {
                    responseList.Add(_mapper.Map<ProjectMilestoneBackerResponse>(item));
                }

                return ResultDTO<List<ProjectMilestoneBackerResponse>>.Success(responseList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
