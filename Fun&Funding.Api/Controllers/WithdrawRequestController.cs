﻿using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.WithdrawDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/withdraw-request")]
    [ApiController]
    public class WithdrawRequestController : ControllerBase
    {
        private readonly IWithdrawService _withdrawService;

        public WithdrawRequestController(IWithdrawService withdrawService)
        {
            _withdrawService = withdrawService;
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRequest()
        {
            var result = await _withdrawService.GetAllRequest();
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRequest(WithdrawReq request)
        {
            var result = await _withdrawService.CreateMarketplaceRequest(request);
            return Ok(result);
        }
        [HttpPatch("{id}/process")]
        public async Task<IActionResult> ProcessingRequest(Guid id)
        {
            var result = await _withdrawService.AdminProcessingRequest(id);
            return Ok(result);
        }
        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> ApprovedRequest(Guid id)
        {
            var result = await _withdrawService.AdminApproveRequest(id);
            return Ok(result);
        }
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelRequest(Guid id)
        {
            var result = await _withdrawService.AdminCancelRequest(id);
            return Ok(result);
        }
        [HttpPost("wallet-request")]
        public async Task<IActionResult> WalletWithdrawRequest()
        {
            var result = await _withdrawService.WalletWithdrawRequest();
            return Ok(result);
        }
    }
}
