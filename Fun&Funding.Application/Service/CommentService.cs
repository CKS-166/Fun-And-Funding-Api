﻿using AutoMapper;
using Fun_Funding.Application.IRepository;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.CommentDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Service
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
        }
        public async Task<ResultDTO<Comment>> CommentProject(CommentRequest request)
        {
            try
            {
                var user = await _userService.GetUserInfo();
                User exitUser = _mapper.Map<User>(user._data);
                var project = await _unitOfWork.FundingProjectRepository.GetAsync(x => x.Id.Equals(request.ProjectId));
                if (user is null)
                {
                    return ResultDTO<Comment>.Fail("User is null");
                }
                if (project is null)
                {
                    return ResultDTO<Comment>.Fail("Project can not found");
                }

                // add new comment
                Comment newComment = new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = request.Content,
                    CreateDate = DateTime.Now,
                    ProjectId = project.Id,
                    UserID = exitUser.Id,
                    IsDelete = false,
                };
                _unitOfWork.commentRepository.Create(newComment);
                return ResultDTO<Comment>.Success(newComment, "Successfully Add Comment");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<List<CommentViewResponse>> GetAllComment()
        {
            try
            {
                var list = _unitOfWork.commentRepository.GetAll();

                List<CommentViewResponse> comments = new List<CommentViewResponse>();

                foreach (var comment in list)
                {
                    // Fetch the user including the file (avatar)
                    var user = _unitOfWork.UserRepository.GetQueryable()
                        .Include(x => x.File) // Include the UserFile
                        .FirstOrDefault(x => x.Id == comment.UserID);

                    // Extract the avatar URL
                    var avatarUrl = user?.File?.URL;

                    comments.Add(new CommentViewResponse
                    {
                        Content = comment.Content,
                        CreateDate = comment.CreateDate,
                        UserName = user?.UserName,  // Ensure safe navigation
                        AvatarUrl = avatarUrl       // Use the extracted URL for avatar
                    });
                }

                return comments;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }
        }

        public async Task<List<CommentViewResponse>> GetCommentsByProject(Guid id)
        {
            try
            {
                var list = _unitOfWork.commentRepository.GetAll().Where(c => c.ProjectId == id);
                List<CommentViewResponse> comments = new List<CommentViewResponse>();
                foreach (var comment in list)
                {
                    // Fetch the user including the file (avatar)
                    var user = _unitOfWork.UserRepository.GetQueryable()
                        .Include(x => x.File) // Include the UserFile
                        .FirstOrDefault(x => x.Id == comment.UserID);

                    // Extract the avatar URL
                    var avatarUrl = user?.File?.URL;

                    comments.Add(new CommentViewResponse
                    {
                        Content = comment.Content,
                        CreateDate = comment.CreateDate,
                        UserName = user?.UserName,  // Ensure safe navigation
                        AvatarUrl = avatarUrl       // Use the extracted URL for avatar
                    });
                }

                return comments;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }
        }
        public async Task<ResultDTO<Comment>> DeleteComment(Guid id)
        {
            //check comment id
            var extiedComment = _unitOfWork.commentRepository.Get(x => x.Id == id);
            var user = await _userService.GetUserInfo();
            User exitUser = _mapper.Map<User>(user._data);
            if (extiedComment is null)
            {
                return ResultDTO<Comment>.Fail("can not find any comment");
            }
            if (!extiedComment.UserID.Equals(user._data.Id))
            {
                return ResultDTO<Comment>.Fail("user are not authorized to do this action");

            }
            try
            {
                var updateComment = Builders<Comment>.Update.Set(x => x.IsDelete, true);
                _unitOfWork.commentRepository.SoftRemove(x => x.Id == extiedComment.Id, updateComment);
                return ResultDTO<Comment>.Success(extiedComment, "Delete Successfull");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
