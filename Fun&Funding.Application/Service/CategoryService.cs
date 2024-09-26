using AutoMapper;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.CategoryDTO;
using Fun_Funding.Domain.Entity;
using System.Linq.Expressions;
using System.Net;

namespace Fun_Funding.Application.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultDTO<CategoryResponse>> CreateCategory(CategoryRequest request)
        {
            try
            {
                var category = _mapper.Map<Category>(request);

                category.CreatedDate = DateTime.Now;
                category.IsDeleted = false;

                await _unitOfWork.CategoryRepository.AddAsync(category);
                await _unitOfWork.CommitAsync();

                var response = _mapper.Map<CategoryResponse>(category);

                return new ResultDTO<CategoryResponse>(true, ["Add successfully."], response, (int)HttpStatusCode.Created);
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

        public async Task<ResultDTO> DeleteCategory(Guid id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

                if (category != null)
                {
                    _unitOfWork.CategoryRepository.Remove(category);
                    _unitOfWork.Commit();

                    return ResultDTO.Success();
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Category Not Found.");
                }
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

        public async Task<ResultDTO<PaginatedResponse<CategoryResponse>>> GetCategories(ListRequest request)
        {
            try
            {
                Expression<Func<Category, bool>> filter = null;
                Expression<Func<Category, object>> orderBy = c => c.CreatedDate;

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

                var list = await _unitOfWork.CategoryRepository.GetAllAsync(
                   filter: filter,
                   orderBy: orderBy,
                   isAscending: request.IsAscending.Value,
                   pageIndex: request.PageIndex,
                   pageSize: request.PageSize);

                if (list != null && list.Count() > 0)
                {
                    var totalItems = _unitOfWork.CategoryRepository.GetAll(filter).Count();
                    var totalPages = (int)Math.Ceiling((double)totalItems / (int)request.PageSize);
                    IEnumerable<CategoryResponse> categories = _mapper.Map<IEnumerable<CategoryResponse>>(list);

                    PaginatedResponse<CategoryResponse> response = new PaginatedResponse<CategoryResponse>
                    {
                        PageSize = request.PageSize.Value,
                        PageIndex = request.PageIndex.Value,
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        Items = categories
                    };

                    return ResultDTO<PaginatedResponse<CategoryResponse>>.Success(response);
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Category Not Found.");
                }
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

        public async Task<ResultDTO<CategoryResponse>> GetCategoryById(Guid id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

                if (category == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Category Not Found.");
                }
                else
                {
                    CategoryResponse response = _mapper.Map<CategoryResponse>(category);
                    return ResultDTO<CategoryResponse>.Success(response);
                }
            }
            catch (Exception ex)
            {
                if (ex is ExceptionError exceptionError)
                {
                    // Rethrow the ExceptionError so it can be handled by the global handler
                    throw exceptionError;
                }

                // For all other exceptions, throw a generic Internal Server Error
                throw new ExceptionError((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ResultDTO<CategoryResponse>> UpdateCategory(Guid id, CategoryRequest request)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

                if (category == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Category Not Found.");
                }
                else
                {
                    _mapper.Map(request, category);

                    _unitOfWork.CategoryRepository.Update(category);
                    await _unitOfWork.CommitAsync();

                    var response = _mapper.Map<CategoryResponse>(category);

                    return new ResultDTO<CategoryResponse>(true, ["Update successfully."], response, (int)HttpStatusCode.OK);

                }
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
