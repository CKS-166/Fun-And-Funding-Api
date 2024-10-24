using AutoMapper;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.FollowDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class FollowService : IFollowService
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public FollowService(IUserService userService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDTO<Follow>> FollowUser(FollowRequest request)
        {
            var user = await _userService.GetUserInfo();
            User exitUser = _mapper.Map<User>(user._data);

            var foundUser = await _userService.GetUserInfoById(request.UserId);
            User userFollowed = _mapper.Map<User>(foundUser._data);

            if (request is null)
            {
                return ResultDTO<Follow>.Fail("can not follow");
            }
            else if (exitUser is null)
            {
                return ResultDTO<Follow>.Fail("user is not authenticated");
            }
            else if (userFollowed is null)
            {
                return ResultDTO<Follow>.Fail("follower is not found");
            }
            var isFollowed = _unitOfWork.FollowRepository.Get(x => x.UserID == exitUser.Id && x.FollowerId == userFollowed.Id);

            try
            {
                if (isFollowed == null)
                {
                    Follow newFollow = new Follow
                    {
                        Id = Guid.NewGuid(),
                        UserID = exitUser.Id,
                        FollowerId = userFollowed.Id,
                        CreateDate = DateTime.Now,
                        IsDelete = false,
                        IsFollow = true,
                    };
                    _unitOfWork.FollowRepository.Create(newFollow);
                    return ResultDTO<Follow>.Success(newFollow, "You have been Followed");
                }
                else
                {
                    if (isFollowed.IsFollow)
                    {
                        var updateFollow = Builders<Follow>.Update
                            .Set(x => x.IsFollow, false)
                            .Set(x => x.IsDelete, true);
                        _unitOfWork.FollowRepository.Update(x => x.Id == isFollowed.Id, updateFollow);
                        var response = _unitOfWork.FollowRepository.Get(x => x.Id == isFollowed.Id);
                        return ResultDTO<Follow>.Success(response, "You just unfollowed");
                    }
                    else
                    {
                        var updateFollow = Builders<Follow>.Update
                            .Set(x => x.IsFollow, true)
                            .Set(x => x.IsDelete, false);
                        _unitOfWork.FollowRepository.Update(x => x.Id == isFollowed.Id, updateFollow);
                        var response = _unitOfWork.FollowRepository.Get(x => x.Id == isFollowed.Id);
                        return ResultDTO<Follow>.Success(response, "You just followed");
                    }
                }
                return ResultDTO<Follow>.Success(isFollowed, "You have been Followed");
            }
            catch (Exception ex)
            {
                return ResultDTO<Follow>.Fail("Something went wrong!");
            }
        }

        public async Task<ResultDTO<List<Follow>>> GetListFollower(Guid UserId)
        {
            try
            {
                var list = _unitOfWork.FollowRepository.GetList(x => x.IsDelete == false).ToList();
                return ResultDTO<List<Follow>>.Success(list, "list follower");
            }
            catch (Exception ex)
            {
                return ResultDTO<List<Follow>>.Fail("Something went wrong!");
            }
        }
    }
}
