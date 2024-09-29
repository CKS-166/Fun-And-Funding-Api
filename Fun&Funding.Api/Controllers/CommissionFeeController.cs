using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.CommissionDTO;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/commision-fees")]
    [ApiController]
    public class CommissionFeeController : ControllerBase
    {
        private readonly ICommissionFeeService _commissionFeeService;

        public CommissionFeeController(ICommissionFeeService commissionFeeService)
        {
            _commissionFeeService = commissionFeeService;
        }

        [HttpGet("applied-commission-fee")]
        public IActionResult GetAppliedCommissionFee([FromQuery] CommissionType type)
        {
            var response = _commissionFeeService.GetAppliedCommissionFee(type);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetCommissionFees([FromQuery] ListRequest request, [FromQuery] CommissionType? type)
        {
            var response = await _commissionFeeService.GetCommissionFees(request, type);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommissionFee([FromBody] CommissionFeeAddRequest request)
        {
            var response = await _commissionFeeService.CreateCommissionFee(request);
            return new ObjectResult(response)
            {
                StatusCode = response._statusCode
            };
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCommissionFee([FromRoute] Guid id, [FromBody] CommissionFeeUpdateRequest request)
        {
            var response = await _commissionFeeService.UpdateCommsisionFee(id, request);
            return Ok(response);
        }
    }
}
