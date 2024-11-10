using AutoMapper;
using Fun_Funding.Application.IRepository;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.LikeDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class LikeService : ILikeService
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public LikeService(IUserService userService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDTO<List<Like>>> CheckIsLike(Guid projectId)
        {
            try
            {
                // Get current user
                var user = await _userService.GetUserInfo();
                User exitUser = _mapper.Map<User>(user._data);
                if (exitUser == null)
                {
                    return ResultDTO<List<Like>>.Fail("Cannot find user");
                }

                // List to store the likes found
                var userLikes = new List<Like>();

                // Check for like in funding project
                var fundingLike = await _unitOfWork.FundingProjectRepository.GetByIdAsync(projectId);
                if (fundingLike != null)
                {
                    var isLike = _unitOfWork.LikeRepository.Get(x => x.ProjectId == fundingLike.Id && x.UserId == exitUser.Id);
                    if (isLike != null) userLikes.Add(isLike);
                }

                // Check for like in marketplace project
                var marketplaceProject = await _unitOfWork.MarketplaceRepository.GetByIdAsync(projectId);
                if (marketplaceProject != null)
                {
                    var isLike = _unitOfWork.LikeRepository.Get(x => x.ProjectId == marketplaceProject.Id && x.UserId == exitUser.Id);
                    if (isLike != null) userLikes.Add(isLike);
                }

                // Return result based on whether any likes were found
                if (userLikes.Any())
                {
                    string message = userLikes.Count > 1 ? "User has liked both projects" : "User has liked one project";
                    return ResultDTO<List<Like>>.Success(userLikes, message);
                }

                return ResultDTO<List<Like>>.Fail("User has not liked any project", 404);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<Like>>.Fail($"Error: {ex.Message}");
            }
        }

        public async Task<List<Like>> CheckUserLike(Guid id)
        {
            try
            {
                var user = _userService.GetUserInfo().Result;
                User exitUser = _mapper.Map<User>(user._data);
                var list = _unitOfWork.LikeRepository.GetAll().Where(l => l.ProjectId == id && l.UserId == exitUser.Id && l.IsDelete == false);
                return list.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Like>> GetAll()
        {
            try
            {
                var list = _unitOfWork.LikeRepository.GetAll();
                var result = list.Where(x => x.IsDelete == false).ToList();
                return list.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }
        }

        public async Task<List<Like>> GetLikesByProject(Guid id)
        {
            try
            {
                var project = _unitOfWork.FundingProjectRepository.GetAsync(x => x.Id.Equals(id));
                if (project is null)
                {
                    throw new Exception("Project can not found");
                }

                var list = _unitOfWork.LikeRepository.GetAll()
                    .Where(l => l.ProjectId == id && l.IsDelete == false)
                    .ToList();
                return list.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }
        }

        public async Task<ResultDTO<LikeResponse>> LikeFundingProject(LikeRequest likeRequest)
        {
            try
            {
                var user = await _userService.GetUserInfo();
                User exitUser = _mapper.Map<User>(user._data);
                var project = await _unitOfWork.FundingProjectRepository.GetAsync(x => x.Id.Equals(likeRequest.ProjectId));
                if (user is null)
                {
                    return ResultDTO<LikeResponse>.Fail("User is null");
                }
                if (project is null)
                {
                    return ResultDTO<LikeResponse>.Fail("Project can not found");
                }
                //check if the user and project already liked 
                var getLikedProjects = _unitOfWork.LikeRepository.Get(x => x.ProjectId.Equals(project.Id) && x.UserId.Equals(exitUser.Id));


                if (getLikedProjects == null)
                {
                    //liked a project
                    Like newLikeProject = new Like
                    {
                        ProjectId = likeRequest.ProjectId,
                        UserId = exitUser.Id,
                        IsLike = true,
                        CreateDate = DateTime.Now,
                        Id = Guid.NewGuid(),
                        IsDelete = false,
                    };
                    _unitOfWork.LikeRepository.Create(newLikeProject);
                    return ResultDTO<LikeResponse>.Success(new LikeResponse { ProjectId = newLikeProject.ProjectId, UserID = newLikeProject.UserId }, "Succesfull like the project");
                }
                else
                {
                    if (getLikedProjects.IsLike == false && getLikedProjects.IsDelete)
                    {
                        var updateDefinition = Builders<Like>.Update.Set(x => x.IsLike, true).Set(x => x.IsDelete, false);
                        _unitOfWork.LikeRepository.Update(x => x.Id == getLikedProjects.Id, updateDefinition);

                        return ResultDTO<LikeResponse>.Success(new LikeResponse { ProjectId = likeRequest.ProjectId, UserID = exitUser.Id }, "Succesfull like the project");
                    }
                    if (getLikedProjects.IsLike && getLikedProjects.IsDelete == false) //isLike == true ? "dislike" : "liked"
                    {
                        var update = Builders<Like>.Update.Set(l => l.IsDelete, true).Set(x => x.IsLike, false);
                        _unitOfWork.LikeRepository.SoftRemove(l => l.Id == getLikedProjects.Id, update);
                        return ResultDTO<LikeResponse>.Success(new LikeResponse { ProjectId = likeRequest.ProjectId, UserID = exitUser.Id }, "Succesfull dislike the project");
                    }
                }

                return ResultDTO<LikeResponse>.Fail("some thing wrong : error ");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<LikeResponse>> LikeMarketplaceProject(LikeRequest likeRequest)
        {
            try
            {
                var user = await _userService.GetUserInfo();
                User exitUser = _mapper.Map<User>(user._data);
                var project = await _unitOfWork.MarketplaceRepository.GetAsync(x => x.Id.Equals(likeRequest.ProjectId));
                if (user is null)
                {
                    return ResultDTO<LikeResponse>.Fail("User is null");
                }
                if (project is null)
                {
                    return ResultDTO<LikeResponse>.Fail("Project can not found");
                }
                //check if the user and project already liked 
                var getLikedProjects = _unitOfWork.LikeRepository.Get(x => x.ProjectId.Equals(project.Id) && x.UserId.Equals(exitUser.Id));


                if (getLikedProjects == null)
                {
                    //liked a project
                    Like newLikeProject = new Like
                    {
                        ProjectId = likeRequest.ProjectId,
                        UserId = exitUser.Id,
                        IsLike = true,
                        CreateDate = DateTime.Now,
                        Id = Guid.NewGuid(),
                        IsDelete = false,
                    };
                    _unitOfWork.LikeRepository.Create(newLikeProject);
                    return ResultDTO<LikeResponse>.Success(new LikeResponse { ProjectId = newLikeProject.ProjectId, UserID = newLikeProject.UserId }, "Succesfull like the project");
                }
                else
                {
                    if (getLikedProjects.IsLike == false && getLikedProjects.IsDelete)
                    {
                        var updateDefinition = Builders<Like>.Update.Set(x => x.IsLike, true).Set(x => x.IsDelete, false);
                        _unitOfWork.LikeRepository.Update(x => x.Id == getLikedProjects.Id, updateDefinition);

                        return ResultDTO<LikeResponse>.Success(new LikeResponse { ProjectId = likeRequest.ProjectId, UserID = exitUser.Id }, "Succesfull like the project");
                    }
                    if (getLikedProjects.IsLike && getLikedProjects.IsDelete == false) //isLike == true ? "dislike" : "liked"
                    {
                        var update = Builders<Like>.Update.Set(l => l.IsDelete, true).Set(x => x.IsLike, false);
                        _unitOfWork.LikeRepository.SoftRemove(l => l.Id == getLikedProjects.Id, update);
                        return ResultDTO<LikeResponse>.Success(new LikeResponse { ProjectId = likeRequest.ProjectId, UserID = exitUser.Id }, "Succesfull dislike the project");
                    }
                }

                return ResultDTO<LikeResponse>.Fail("some thing wrong : error ");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
