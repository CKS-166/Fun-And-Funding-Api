using AutoMapper;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.CategoryDTO;
using Fun_Funding.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class MarketplaceService : IMarketplaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public MarketplaceService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<ResultDTO<PaginatedResponse<MarketplaceProject>>> GetAllMarketplaceProject(ListRequest request)
        {

            try
            {
                Expression<Func<MarketplaceProject, bool>> filter = null;
                Expression<Func<MarketplaceProject, object>> orderBy = c => c.CreatedDate;

                if (!string.IsNullOrEmpty(request.OrderBy))
                {
                    switch (request.OrderBy.ToLower())
                    {
                        case "name":
                            orderBy = c => c.Name;
                            break;
                        default:
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    filter = c => c.Name.ToLower().Contains(request.SearchValue.ToLower());
                }

                var list = await _unitOfWork.MarketplaceRepository.GetAllAsync(
                   filter: filter,
                   orderBy: orderBy,
                   isAscending: request.IsAscending.Value,
                   pageIndex: request.PageIndex,
                   pageSize: request.PageSize);

                if (list != null && list.Count() > 0)
                {
                    var totalItems = _unitOfWork.MarketplaceRepository.GetAll(filter).Count();
                    var totalPages = (int)Math.Ceiling((double)totalItems / (int)request.PageSize);
                    IEnumerable<MarketplaceProject> categories = _mapper.Map<IEnumerable<MarketplaceProject>>(list);

                    PaginatedResponse<MarketplaceProject> response = new PaginatedResponse<MarketplaceProject>
                    {
                        PageSize = request.PageSize.Value,
                        PageIndex = request.PageIndex.Value,
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        Items = categories
                    };

                    return ResultDTO<PaginatedResponse<MarketplaceProject>>.Success(response);
                }
                else
                {
                    return ResultDTO<PaginatedResponse<MarketplaceProject>>.Fail("Marketplace Not Found");
                }

            }
            catch (Exception ex)
            {
                return ResultDTO<PaginatedResponse<MarketplaceProject>>.Fail("Something wrong");
            }
        }
    }
}
