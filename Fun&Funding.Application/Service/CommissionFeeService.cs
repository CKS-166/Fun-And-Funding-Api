using AutoMapper;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.CommissionDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using System.Net;

namespace Fun_Funding.Application.Service
{
    public class CommissionFeeService : ICommissionFeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommissionFeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultDTO<CommissionFeeResponse>> CreateCommissionFee(CommissionFeeAddRequest request)
        {
            try
            {
                var commission = _mapper.Map<CommissionFee>(request);
                commission.CreatedDate = commission.UpdateDate = DateTime.Now;

                _unitOfWork.CommissionFeeRepository.Add(commission);
                await _unitOfWork.CommitAsync();

                var response = _mapper.Map<CommissionFeeResponse>(commission);

                return new ResultDTO<CommissionFeeResponse>(true, ["Create successfully."], response, (int)HttpStatusCode.Created);
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

        public ResultDTO<CommissionFeeResponse> GetAppliedCommissionFee(CommissionType type)
        {
            try
            {
                var commissionFee = _unitOfWork.CommissionFeeRepository.GetQueryable()
                                .Where(c => c.CommissionType == type)
                                .OrderByDescending(c => c.UpdateDate)
                                .FirstOrDefault();

                if (commissionFee != null)
                {
                    var response = _mapper.Map<CommissionFeeResponse>(commissionFee);

                    return ResultDTO<CommissionFeeResponse>.Success(response);
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "No Commission Fee in Database.");
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

        public async Task<ResultDTO<CommissionFeeResponse>> UpdateCommsisionFee(Guid id, CommissionFeeUpdateRequest request)
        {
            try
            {
                var commission = await _unitOfWork.CommissionFeeRepository.GetByIdAsync(id);

                if (commission != null)
                {
                    commission.Id = new Guid();
                    commission.Rate = request.Rate;
                    commission.UpdateDate = DateTime.Now;

                    _unitOfWork.CommissionFeeRepository.Add(commission);
                    await _unitOfWork.CommitAsync();

                    var response = _mapper.Map<CommissionFeeResponse>(commission);

                    return ResultDTO<CommissionFeeResponse>.Success(response);
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Commission Fee not found.");
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
