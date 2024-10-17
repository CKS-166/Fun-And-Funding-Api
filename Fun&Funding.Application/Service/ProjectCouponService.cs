using AutoMapper;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Domain.Entity;
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

        public async Task<ResultDTO<List<ProjectCoupon>>> ImportFile(IFormFile formFile, Guid projectId)
        {
            try
            {
                var couponList = new List<ProjectCoupon>();

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
                                    IsDeleted = false,
                                    //MarketplaceProjectId = projectId,
                                    //MarketplaceProject = await _unitOfWork.MarketplaceRepository.GetByIdAsync(projectId),
                                };
                                couponList.Add(coupon);
                            };
                        }
                    }
                }
                await _unitOfWork.ProjectCouponRepository.AddRangeAsync(couponList);
                await _unitOfWork.CommitAsync();
                return ResultDTO<List<ProjectCoupon>>.Success(couponList, "Successfully add couponList");
            }
            catch (Exception ex)
            {
                return ResultDTO<List<ProjectCoupon>>.Fail($"{ex.Message}");
            }
        }
    }
}
