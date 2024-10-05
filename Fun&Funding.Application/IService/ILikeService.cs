using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.LikeDTO;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface ILikeService
    {
        Task<List<Like>> GetAll();
        Task<ResultDTO<LikeResponse>> LikeProject(LikeRequest likeRequest);
        Task<List<Like>> GetLikesByProject(Guid id);
        Task<List<Like>> CheckUserLike(Guid id);
    }
}
