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
    }
}
