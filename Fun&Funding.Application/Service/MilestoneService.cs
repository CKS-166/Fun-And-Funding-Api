using AutoMapper;
using Azure.Core;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.MilestoneDTO;
using Fun_Funding.Domain.Constrain;
using Fun_Funding.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Service
{
    public class MilestoneService : IMilestoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public MilestoneService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<ResultDTO<MilestoneResponse>> CreateMilestone(AddMilestoneRequest request)
        {
            var user = _userService.GetUserInfo().Result;
            User exitUser = _mapper.Map<User>(user._data);
            if (user is null)
            {
                return ResultDTO<MilestoneResponse>.Fail("can not found user");
            }
            if (_userService.CheckUserRole(exitUser).Result.ToString() == Role.Admin)
            {
                return ResultDTO<MilestoneResponse>.Fail("user is not admin");
            }
            var latestMilestone = _unitOfWork.MilestoneRepository.GetQueryable()
                    .Where(x => x.MilestoneOrder.Equals(request.MilestoneOrder))
                    .OrderByDescending(x => x.Version)
                    .FirstOrDefault();
            try
            {
                if (latestMilestone == null)
                {
                    //create new Milestone
                    Milestone milestone = new Milestone
                    {
                        Id = Guid.NewGuid(),
                        Version = 1,
                        CreatedDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Description = request.Description,
                        DisbursementPercentage = request.DisbursementPercentage,
                        Duration = request.Duration,
                        MilestoneName = request.MilestoneName,
                        MilestoneOrder = request.MilestoneOrder,
                        IsDeleted = false,
                    };
                    await _unitOfWork.MilestoneRepository.AddAsync(milestone);
                    await _unitOfWork.CommitAsync();
                    var response = _mapper.Map<MilestoneResponse>(milestone);
                    return ResultDTO<MilestoneResponse>.Success(response, "Success");
                }
                else
                {
                    Milestone milestone = new Milestone
                    {
                        Id = Guid.NewGuid(),
                        Version = latestMilestone.Version + 1,
                        CreatedDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Description = request.Description,
                        DisbursementPercentage = request.DisbursementPercentage,
                        Duration = request.Duration,
                        MilestoneName = request.MilestoneName,
                        MilestoneOrder = request.MilestoneOrder,
                        IsDeleted = false,
                    };
                    await _unitOfWork.MilestoneRepository.AddAsync(milestone);
                    //update soft delete
                    latestMilestone.IsDeleted = true;
                    latestMilestone.DeletedAt = DateTime.Now;
                    _unitOfWork.MilestoneRepository.Update(latestMilestone);
                    await _unitOfWork.CommitAsync();
                    var response = _mapper.Map<MilestoneResponse>(milestone);
                    return ResultDTO<MilestoneResponse>.Success(response, "Success");
                }


            }
            catch (Exception ex)
            {
                return ResultDTO<MilestoneResponse>.Fail("something wrong");
            }
        }

        public async Task<ResultDTO<List<MilestoneResponse>>> GetListLastestMilestone()
        {
            var user = _userService.GetUserInfo().Result;
            User exitUser = _mapper.Map<User>(user._data);
            if (user is null)
            {
                return ResultDTO<List<MilestoneResponse>>.Fail("can not found user");
            }
            try
            {
                var latestGroupMilestones = _unitOfWork.MilestoneRepository.GetQueryable()
                    .GroupBy(x => x.MilestoneOrder)
                    .Select(g => g.OrderByDescending(x => x.Version).FirstOrDefault())
                    .ToList();
                List<MilestoneResponse> responses = _mapper.Map<List<MilestoneResponse>>(latestGroupMilestones);
                return ResultDTO<List<MilestoneResponse>>.Success(responses, "Group latest milestone");
            }
            catch (Exception ex)
            {
                return ResultDTO<List<MilestoneResponse>>.Fail("something wrong");
            }
        }



        public async Task<ResultDTO<List<MilestoneResponse>>> GetMilestoneByVersionAndOrder(int? Order, int? Version)
        {
            var user = await _userService.GetUserInfo();
            if (user?._data is null)
            {
                return ResultDTO<List<MilestoneResponse>>.Fail("Cannot find user");
            }

            User exitUser = _mapper.Map<User>(user._data);

            try
            {
                var query = _unitOfWork.MilestoneRepository.GetQueryable();

                List<Milestone> milestones;

                if (Order is null && Version.HasValue)
                {
                    // Get all milestones with the specific Version
                    milestones = query.Where(x => x.Version == Version).ToList();
                }
                else if (Version is null && Order.HasValue)
                {
                    // Get all milestones with the specific Order
                    milestones = query.Where(x => x.MilestoneOrder == Order).ToList();
                }
                else if (Order.HasValue && Version.HasValue)
                {
                    // Get milestones matching both Order and Version
                    milestones = query.Where(x => x.MilestoneOrder == Order && x.Version == Version).ToList();
                }
                else
                {
                    // If both are null, return an empty list
                    milestones = new List<Milestone>();
                }

                if (!milestones.Any())
                {
                    return ResultDTO<List<MilestoneResponse>>.Fail("No milestones found");
                }

                var response = _mapper.Map<List<MilestoneResponse>>(milestones);
                return ResultDTO<List<MilestoneResponse>>.Success(response, "Fetched milestones successfully");
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return ResultDTO<List<MilestoneResponse>>.Fail("Something went wrong");
            }
        }
    }
}
