using AutoMapper;
using Fun_Funding.Application.Interfaces.IEntityService;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.FeedbackDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<ResultDTO<Feedback>> ApprovedById(Guid id)
        {
            try
            {
                var feedback = GetFeedbackById(id).Result._data;
                feedback.Status = true;
                _unitOfWork.FeedbackRepository.Update(x => x.Id == feedback.Id, Builders<Feedback>.Update.Set(x => x.Status, true));
                await _unitOfWork.CommitAsync();
                return ResultDTO<Feedback>.Success(feedback);
            }
            catch (Exception ex)
            {
                return ResultDTO<Feedback>.Fail($"somethings wrong: {ex.Message}");
            }
        }

        public async Task<ResultDTO<Feedback>> CreateFeedBack(FeedbackRequest request)
        {
            try
            {
                var user = await _userService.GetUserInfo();
                User existUser = _mapper.Map<User>(user._data);
                if (existUser is null)
                {
                    return ResultDTO<Feedback>.Fail("No User found");
                }
                var feedback = new Feedback
                {
                    Id = Guid.NewGuid(),
                    Content = request.Content,
                    CreateDate = DateTime.Now,
                    IsDelete = false,
                    UserID = existUser.Id,
                    Status = false,
                };
                _unitOfWork.FeedbackRepository.Create(feedback);
                await _unitOfWork.CommitAsync();
                return ResultDTO<Feedback>.Success(feedback, "successfull create feedback");
            }
            catch (Exception ex)
            {
                return ResultDTO<Feedback>.Fail($"something wrongs: {ex.Message}");
            }
        }

        public async Task<ResultDTO<List<Feedback>>> Get4RandomFeedback()
        {
            try
            {
                // Get total count first to handle edge cases
                var totalCount = _unitOfWork.FeedbackRepository.GetQueryable().Count();

                // Handle case where there are fewer than 4 items
                var itemsToTake = Math.Min(4, totalCount);

                if (totalCount == 0)
                {
                    return ResultDTO<List<Feedback>>.Success(new List<Feedback>());
                }

                // Get all IDs
                var allIds = _unitOfWork.FeedbackRepository.GetQueryable()
                    .Select(f => f.Id)
                    .ToList();

                // Use modern RandomNumberGenerator
                using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
                var randomIds = allIds.OrderBy(id =>
                {
                    byte[] randomBytes = new byte[4];
                    rng.GetBytes(randomBytes);
                    return BitConverter.ToInt32(randomBytes, 0);
                })
                .Take(itemsToTake)
                .ToList();

                // Get the randomly selected feedback items
                var feedbackList = _unitOfWork.FeedbackRepository.GetQueryable()
                    .Where(f => randomIds.Contains(f.Id) && f.Status)
                    .ToList();

                return ResultDTO<List<Feedback>>.Success(feedbackList);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<Feedback>>.Fail($"Something went wrong: {ex.Message}");
            }
        }

        public async Task<ResultDTO<PaginatedResponse<Feedback>>> GetAllFeedback(ListRequest request)
        {
            try
            {
                var list = _unitOfWork.FeedbackRepository.GetAllPaged(request);
                return ResultDTO<PaginatedResponse<Feedback>>.Success(list, "Successfull querry");
            }
            catch (Exception ex)
            {
                return ResultDTO<PaginatedResponse<Feedback>>.Fail("something wrong!");
            }
        }

        public async Task<ResultDTO<Feedback>> GetFeedbackById(Guid id)
        {
            try
            {
                var exitedFeedback = _unitOfWork.FeedbackRepository.Get(x => x.Id == id);
                if (exitedFeedback == null) return ResultDTO<Feedback>.Fail($"not found any feedback");
                return ResultDTO<Feedback>.Success(exitedFeedback);
            }
            catch (Exception ex)
            {
                return ResultDTO<Feedback>.Fail($"somethings wrong: {ex.Message}");
            }
        }
    }
}
