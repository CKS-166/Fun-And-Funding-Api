using AutoMapper;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.Interfaces.IExternalServices;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.MarketplaceFileDTO;
using Fun_Funding.Application.ViewModel.MarketplaceProjectDTO;
using Fun_Funding.Domain.Entity;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class MarketplaceService : IMarketplaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IFundingProjectService _fundingProjectService;
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly IAzureService _azureService;

        public MarketplaceService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService,
            IHttpContextAccessor httpContextAccessor, IFundingProjectService fundingProjectService,
            IAzureService azureService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
            _claimsPrincipal = httpContextAccessor.HttpContext.User;
            _fundingProjectService = fundingProjectService;
            _azureService = azureService;
        }

        public async Task<ResultDTO<MarketplaceProjectInfoResponse>> CreateMarketplaceProject(MarketplaceProjectAddRequest request)
        {
            try
            {
                //find funding project
                var fundingProject = await _unitOfWork.FundingProjectRepository
                    .GetByIdAsync(request.FundingProjectId);
                if (fundingProject == null)
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Funding Project not found.");

                //validate
                var errorMessages = validateAddMarketplaceProject(request);
                if (errorMessages != null && errorMessages.Count > 0)
                {
                    throw new ExceptionError((int)HttpStatusCode.BadRequest, string.Join("\n", errorMessages));
                }

                //add files 
                List<MarketplaceFile> files = new List<MarketplaceFile>();

                foreach (MarketplaceFileAddRequest file in request.MarketplaceFiles)
                {
                    if (file.URL.Length > 0)
                    {
                        var result = _azureService.UploadUrlSingleFiles(file.URL);

                        if (result == null)
                        {
                            throw new ExceptionError((int)HttpStatusCode.BadRequest, "Fail to upload file");
                        }

                        MarketplaceFile media = new MarketplaceFile
                        {
                            Name = file.Name,
                            URL = result.Result,
                            FileType = file.FileType
                        };

                        files.Add(media);
                    }
                }

                //map project
                var marketplaceProject = _mapper.Map<MarketplaceProject>(request);
                marketplaceProject.MarketplaceFiles = files;
                marketplaceProject.CreatedDate = DateTime.Now;

                //save to db
                await _unitOfWork.MarketplaceRepository.AddAsync(marketplaceProject);
                await _unitOfWork.CommitAsync();

                //response
                var response = _mapper.Map<MarketplaceProjectInfoResponse>(marketplaceProject);
                return new ResultDTO<MarketplaceProjectInfoResponse>(true, "Create successfully.",
                    response, (int)HttpStatusCode.Created);
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

        private List<string> validateAddMarketplaceProject(MarketplaceProjectAddRequest request)
        {
            try
            {
                List<string> errorMessages = new List<string>();

                if (string.IsNullOrEmpty(request.Name))
                {
                    errorMessages.Add("Name is required.");
                }

                if (string.IsNullOrEmpty(request.Description))
                {
                    errorMessages.Add("Description is required.");
                }

                if (string.IsNullOrEmpty(request.Introduction))
                {
                    errorMessages.Add("Introduction is required.");
                }

                if (request.Price <= 0)
                {
                    errorMessages.Add("Invalid price.");
                }

                if (request.MarketplaceFiles.Count <= 0)
                {
                    errorMessages.Add("Media file(s) is required.");
                }

                return errorMessages;
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
