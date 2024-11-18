using Fun_Funding.Application.Interfaces.IEntityService;
using Fun_Funding.Application.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/system-wallet")]
    [ApiController]
    public class SystemWalletController : ControllerBase
    {
        private ISystemWalletService _systemWalletService;
        public SystemWalletController(ISystemWalletService systemWalletService)
        {
            _systemWalletService = systemWalletService;
        }

        [HttpGet("platform-revenue")]
        public async Task<IActionResult> GetPlatformRevenue()
        {
            var response = await _systemWalletService.GetPlatformRevenue();
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWallet()
        {
            var res = await _systemWalletService.CreateWallet();
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetWallet()
        {
            var res = _systemWalletService.GetSystemWallet();
            return Ok(res);
        }
    }
}
