﻿using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.CommissionDTO;
using Fun_Funding.Domain.Enum;

namespace Fun_Funding.Application.IService
{
    public interface ICommissionFeeService
    {
        ResultDTO<CommissionFeeResponse> GetAppliedCommissionFee(CommissionType type);
        Task<ResultDTO<CommissionFeeResponse>> UpdateCommsisionFee(Guid id, CommissionFeeUpdateRequest request);
        Task<ResultDTO<CommissionFeeResponse>> CreateCommissionFee(CommissionFeeAddRequest request);
    }
}
