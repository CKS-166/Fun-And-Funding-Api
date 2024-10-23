using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.FollowDTO;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IFollowService
    {
        Task<ResultDTO<Follow>> FollowUser(FollowRequest request);
        Task<ResultDTO<List<Follow>>> GetListFollower(Guid UserId);
    }
}
