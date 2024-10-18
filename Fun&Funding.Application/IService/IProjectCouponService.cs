using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.CouponDTO;
using Fun_Funding.Domain.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IProjectCouponService
    {
        public List<ProjectCoupon> CheckDuplicateCouponCode(Guid? marketplaceId, List<ProjectCoupon> list);
        public Task<ResultDTO<ListCouponResponse>> ImportFile(IFormFile formFile, Guid projectId);
        public Task<ResultDTO<ProjectCoupon>> ChangeStatus(Guid couponId);
    }
}
