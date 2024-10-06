using AutoMapper;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.UserDTO;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Fun_Funding.Application.ExceptionHandler;
using System.Net;
using Fun_Funding.Domain.Enum;
using Fun_Funding.Application.ViewModel.CategoryDTO;
using System.Linq.Expressions;

namespace Fun_Funding.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserService(IUnitOfWork unitOfWork, 
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _claimsPrincipal = httpContextAccessor.HttpContext.User;
            _mapper = mapper;
        }
        public async Task<ResultDTO<PaginatedResponse<UserInfoResponse>>> GetUsers(ListRequest request)
        {
            try
            {
                Expression<Func<User, bool>> filter = null;
                Expression<Func<User, object>> orderBy = u => u.CreatedDate;
                const string excludeRoleName = "Administrator";

                IQueryable<User> usersQuery = _unitOfWork.UserRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(u => u.Wallet);

                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    string searchLower = request.SearchValue.ToLower();
                    filter = u =>
                        (u.FullName != null && u.FullName.ToLower().Contains(searchLower)) ||
                        (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                        (u.UserName != null && u.UserName.ToLower().Contains(searchLower));
                }

                var excludeRole = await _roleManager.FindByNameAsync(excludeRoleName);
                if (excludeRole != null)
                {
                    var userIdsInExcludeRole = await _userManager.GetUsersInRoleAsync(excludeRoleName);
                    var excludeUserIds = userIdsInExcludeRole.Select(u => u.Id).ToList();
                    usersQuery = usersQuery.Where(u => !excludeUserIds.Contains(u.Id));
                }

                if (!string.IsNullOrEmpty(request.OrderBy))
                {
                    switch (request.OrderBy.ToLower())
                    {
                        case "fullname":
                            orderBy = u => u.FullName;
                            break;
                        case "email":
                            orderBy = u => u.Email;
                            break;
                        default:
                            break;
                    }
                }

                var totalItems = await usersQuery.CountAsync();
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;

                var paginatedUsers = await usersQuery
                    .OrderBy(orderBy)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(request.PageSize ?? 10)
                    .ToListAsync();

                var usersList = _mapper.Map<IEnumerable<UserInfoResponse>>(paginatedUsers);

                var totalPages = (int)Math.Ceiling((double)totalItems / (request.PageSize ?? 10));

                var paginatedResponse = new PaginatedResponse<UserInfoResponse>
                {
                    PageSize = request.PageSize ?? 10,
                    PageIndex = request.PageIndex ?? 1,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Items = usersList
                };

                return ResultDTO<PaginatedResponse<UserInfoResponse>>.Success(paginatedResponse, "User found!");
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

        private async Task<List<User>> GetUsersByRoleAsync(string roleName)
        {
            try
            {
                var usersInRole = new List<User>();

                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    var users = _userManager.Users.ToList();
                    foreach (var user in users)
                    {
                        if (await _userManager.IsInRoleAsync(user, roleName))
                        {
                            usersInRole.Add(user);
                        }
                    }
                }

                return usersInRole;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionError exceptionError)
                {
                    throw exceptionError;
                }
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<ResultDTO<UserInfoResponse>> GetUserInfo()
        {
            try
            {
                if (_claimsPrincipal == null || !_claimsPrincipal.Identity.IsAuthenticated)
                {
                    throw new ExceptionError((int)HttpStatusCode.Unauthorized, "User not authenticated.");
                }
                var userEmailClaims = _claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (userEmailClaims == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }
                var userEmail = userEmailClaims.Value;
                var user = await _unitOfWork.UserRepository.GetQueryable()
                                .AsNoTracking()
                                .Include(u => u.Wallet)
                                .FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }
                var userInfoResponse = _mapper.Map<UserInfoResponse>(user);

                return ResultDTO<UserInfoResponse>.Success(userInfoResponse, "User found!");
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
        public async Task<ResultDTO<UserInfoResponse>> GetUserInfoById(Guid id)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetQueryable()
                                .Include(u => u.Wallet)
                                .FirstOrDefaultAsync(u => u.Id == id);
                if (user is null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }
                var userResponse = _mapper.Map<UserInfoResponse>(user);
                return ResultDTO<UserInfoResponse>.Success(userResponse, "User found!");

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
        public async Task<ResultDTO<string>> UpdateUser(UserUpdateRequest userUpdateRequest)
        {
            try
            {
                if (_claimsPrincipal == null || !_claimsPrincipal.Identity.IsAuthenticated)
                {
                    throw new ExceptionError((int)HttpStatusCode.Unauthorized, "User not authenticated.");
                }
                var userEmailClaims = _claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (userEmailClaims == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }
                var userEmail = userEmailClaims.Value;
                User user = await _unitOfWork.UserRepository.GetAsync(x => x.Email == userEmail);
                if (user == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }
                if (userUpdateRequest.DayOfBirth != null)
                {
                    var tenYearsAgo = DateTime.Today.AddYears(-10);
                    if (userUpdateRequest.DayOfBirth > tenYearsAgo)
                    {
                        throw new ExceptionError((int)HttpStatusCode.BadRequest, "User must be at least 10 years old.");
                    }
                }
                _mapper.Map(userUpdateRequest, user);
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.CommitAsync();

                return ResultDTO<string>.Success("", "Update Successfully");
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
        public async Task<ResultDTO<UserInfoResponse>> ChangeUserStatus(Guid id)
        {
            try
            {
                var parseUser = await _unitOfWork.UserRepository.GetQueryable()
                                .Include(u => u.Wallet)
                                .FirstOrDefaultAsync(u => u.Id == id);
                if (parseUser is null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }
                UserInfoResponse userResponse = new UserInfoResponse();
                if (parseUser.UserStatus == UserStatus.Active)
                {
                    parseUser.UserStatus = UserStatus.Inactive;
                    await _unitOfWork.CommitAsync();
                    parseUser = await _userManager.FindByIdAsync(id.ToString());
                    userResponse = _mapper.Map<UserInfoResponse>(parseUser);
                    return ResultDTO<UserInfoResponse>.Success(userResponse, "Status changed successfully to Inactive");
                }
                parseUser.UserStatus = UserStatus.Active;
                await _unitOfWork.CommitAsync();
                parseUser = await _userManager.FindByIdAsync(id.ToString());
                userResponse = _mapper.Map<UserInfoResponse>(parseUser);
                return ResultDTO<UserInfoResponse>.Success(userResponse, "Status changed successfully to Active");
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
        public async Task<ResultDTO<string>> ChangeUserPassword(UserChangePasswordRequest userChangePasswordRequest)
        {
            try
            {
                if (_claimsPrincipal == null || !_claimsPrincipal.Identity.IsAuthenticated)
                {
                    throw new ExceptionError((int)HttpStatusCode.Unauthorized, "User not authenticated.");
                }

                var userEmailClaims = _claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (userEmailClaims == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }

                var userEmail = userEmailClaims.Value;
                User user = await _unitOfWork.UserRepository.GetAsync(x => x.Email == userEmail);
                if (user == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }

                if (userChangePasswordRequest.NewPassword != userChangePasswordRequest.ConfirmPassword)
                {
                    throw new ExceptionError((int)HttpStatusCode.BadRequest, "New password and confirmation password do not match.");
                }

                var result = await _userManager.ChangePasswordAsync(user, userChangePasswordRequest.OldPassword, userChangePasswordRequest.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new ExceptionError((int)HttpStatusCode.BadRequest, $"Password change failed: {errors}");
                }

                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.CommitAsync();

                return ResultDTO<string>.Success("", "Password updated successfully");
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
