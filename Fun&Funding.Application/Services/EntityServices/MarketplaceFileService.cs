﻿using AutoMapper;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.Interfaces.IEntityService;
using Fun_Funding.Application.Interfaces.IExternalServices;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.MarketplaceFileDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using System.Linq.Expressions;
using System.Net;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class MarketplaceFileService : IMarketplaceFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAzureService _azureService;

        public MarketplaceFileService(IUnitOfWork unitOfWork, IMapper mapper, IAzureService azureService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _azureService = azureService;
        }

        public async Task<ResultDTO<MarketplaceFileInfoResponse>> UploadGameUpdateFile
            (Guid marketplaceProjectId, MarketplaceGameFileRequest request)
        {
            try
            {
                var errorMessages = validateCommonFields(request);

                if (!(errorMessages.Count > 0))
                {
                    var result = _azureService.UploadUrlSingleFiles(request.URL);

                    if (result == null)
                    {
                        throw new ExceptionError((int)HttpStatusCode.BadRequest, "Fail to upload file");
                    }

                    MarketplaceFile file = new MarketplaceFile
                    {
                        Name = request.Name,
                        URL = result.Result,
                        FileType = FileType.GameFile,
                        CreatedDate = DateTime.Now,
                        MarketplaceProjectId = marketplaceProjectId,
                        Description = request.Description,
                        Version = request.Version
                    };

                    await _unitOfWork.MarketplaceFileRepository.AddAsync(file);
                    await _unitOfWork.CommitAsync();

                    var response = _mapper.Map<MarketplaceFileInfoResponse>(file);

                    return new ResultDTO<MarketplaceFileInfoResponse>(true, "Create successfully.",
                        response, (int)HttpStatusCode.Created);
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.BadRequest, string.Join("\n", errorMessages));
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

        public async Task<ResultDTO<PaginatedResponse<MarketplaceFileInfoResponse>>> GetGameFiles
            (Guid marketplaceProjectId, ListRequest request)
        {
            try
            {
                Expression<Func<MarketplaceFile, bool>> filter = f =>
                (f.MarketplaceProjectId == marketplaceProjectId) && (f.FileType == FileType.GameFile);
                Expression<Func<MarketplaceFile, object>> orderBy = f => f.CreatedDate;

                if (!string.IsNullOrEmpty(request.OrderBy))
                {
                    switch (request.OrderBy.ToLower())
                    {
                        case "version":
                            orderBy = f => f.Version;
                            break;
                        default:
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    filter = f => f.Version.ToLower().Contains(request.SearchValue.ToLower());
                }

                var list = await _unitOfWork.MarketplaceFileRepository.GetAllAsync(
                   filter: filter,
                   orderBy: orderBy,
                   isAscending: request.IsAscending.Value,
                   pageIndex: request.PageIndex,
                   pageSize: request.PageSize);

                if (list != null && list.Count() > 0)
                {
                    var totalItems = _unitOfWork.MarketplaceFileRepository.GetAll(filter).Count();
                    var totalPages = (int)Math.Ceiling((double)totalItems / (int)request.PageSize);
                    IEnumerable<MarketplaceFileInfoResponse> marketplaceFiles =
                        _mapper.Map<IEnumerable<MarketplaceFileInfoResponse>>(list);

                    PaginatedResponse<MarketplaceFileInfoResponse> response = new PaginatedResponse<MarketplaceFileInfoResponse>
                    {
                        PageSize = request.PageSize.Value,
                        PageIndex = request.PageIndex.Value,
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        Items = marketplaceFiles
                    };

                    return ResultDTO<PaginatedResponse<MarketplaceFileInfoResponse>>.Success(response);
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "File(s) not found.");
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

        //validation upload game update file
        private List<string> validateCommonFields(MarketplaceGameFileRequest request)
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

                if (string.IsNullOrEmpty(request.Version))
                {
                    errorMessages.Add("Version is required.");
                }

                if (request.URL.Length <= 0)
                {
                    errorMessages.Add("File is required.");
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
    }
}
