using Fun_Funding.Application.ViewModel.UserDTO;
using Fun_Funding.Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IUserService
    {
        Task<ResultDTO<PaginatedResponse<UserInfoResponse>>> GetUsers(ListRequest request);
        Task<ResultDTO<UserInfoResponse>> GetUserInfo();
        Task<ResultDTO<UserInfoResponse>> GetUserInfoById(Guid id);
        Task<ResultDTO<string>> UpdateUser(UserUpdateRequest userUpdateRequest);
        Task<ResultDTO<UserInfoResponse>> ChangeUserStatus(Guid id);
    }
}
