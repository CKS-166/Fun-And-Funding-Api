using AutoMapper;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.CouponDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.IO;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace Fun_Funding.Application.Service
{
    public class ProjectCouponService : IProjectCouponService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public ProjectCouponService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<ResultDTO<ProjectCoupon>> ChangeStatus(Guid couponId)
        {
            var exitedCoupon = await _unitOfWork.ProjectCouponRepository.GetByIdAsync(couponId);
            if (exitedCoupon == null)
                return ResultDTO<ProjectCoupon>.Fail($"Coupon not found");
            if (exitedCoupon.IsDeleted)
                return ResultDTO<ProjectCoupon>.Fail($"Coupon is delete");

            try
            {
                if (exitedCoupon.Status.Equals(ProjectCouponStatus.Disable))
                {
                    exitedCoupon.Status = ProjectCouponStatus.Enable;
                    _unitOfWork.ProjectCouponRepository.Update(exitedCoupon);
                    await _unitOfWork.CommitAsync();
                    return ResultDTO<ProjectCoupon>.Success(exitedCoupon,$"Coupon is Enable");
                }
                else
                {
                    exitedCoupon.Status = ProjectCouponStatus.Disable;
                    _unitOfWork.ProjectCouponRepository.Update(exitedCoupon);
                    await _unitOfWork.CommitAsync();
                    return ResultDTO<ProjectCoupon>.Success(exitedCoupon,$"Coupon is Disable");
                }
            }
            catch (Exception ex)
            {
                return ResultDTO<ProjectCoupon>.Fail($"{ex.Message}");
            }
        }

        public List<ProjectCoupon> CheckDuplicateCouponCode(Guid? marketplaceId, List<ProjectCoupon> list)
        {
            try
            {
                var listExited = _unitOfWork.ProjectCouponRepository
                    .GetAll(x => x.MarketplaceProjectId.Equals(marketplaceId)).ToList();
                var newList = new List<ProjectCoupon>();
                foreach (var coupon in list)
                {
                    if (listExited.All(x => x.CouponKey != coupon.CouponKey || x.IsDeleted || x.Status.Equals(ProjectCouponStatus.Disable)))
                    {
                        newList.Add(coupon);
                    }
                }
                return newList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<ListCouponResponse>> ImportFile(IFormFile formFile, Guid projectId)
        {
            try
            {
                var couponList = new List<ProjectCoupon>();
                var listCouponMap = new List<CouponResponse>();

                //open memory stream for reading .xls
                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    stream.Position = 0;

                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    using (var workbook = new XSSFWorkbook(stream))
                    {
                        var sheet = workbook.GetSheetAt(0);
                        for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                        {
                            var row = sheet.GetRow(rowIndex);
                            if (row != null)
                            {
                                //taking Coupon
                                var coupon = new ProjectCoupon
                                {
                                    Id = Guid.NewGuid(),
                                    CouponKey = row.GetCell(0)?.ToString() ?? string.Empty, // Check for null and provide a default value
                                    CouponName = row.GetCell(1)?.ToString() ?? string.Empty, // Check for null and provide a default value
                                    CommissionRate = decimal.TryParse(row.GetCell(2)?.ToString(), out var commissionRate) ? commissionRate : 0, // Try parsing safely
                                    CreatedDate = DateTime.Now,
                                    ExpiredDate = DateTime.Now.AddDays(30),
                                    Status = Domain.Enum.ProjectCouponStatus.Enable,
                                    IsDeleted = false,
                                    MarketplaceProjectId = projectId,
                                    MarketplaceProject = await _unitOfWork.MarketplaceRepository.GetByIdAsync(projectId),
                                };
                                couponList.Add(coupon);
                            };
                        }
                        //check COUPON_CODE trùng hoặc disable (Disctin)
                        var ListChecked = CheckDuplicateCouponCode(projectId, couponList);
                        listCouponMap = _mapper.Map<List<CouponResponse>>(ListChecked);
                        var response = new ListCouponResponse
                        {
                            numOfCoupon = listCouponMap.Count,
                            List = listCouponMap
                        };
                        await _unitOfWork.ProjectCouponRepository.AddRangeAsync(ListChecked);
                        await _unitOfWork.CommitAsync();
                        return ResultDTO<ListCouponResponse>.Success(response, "Successfully add couponList");
                    }
                }
            }
            catch (Exception ex)
            {
                return ResultDTO<ListCouponResponse>.Fail($"{ex.Message}");
            }
        }
    }
}
