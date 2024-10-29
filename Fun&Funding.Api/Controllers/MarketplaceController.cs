using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.MarketplaceProjectDTO;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/marketplace-projects")]
    [ApiController]
    public class MarketplaceController : ControllerBase
    {
        private readonly IMarketplaceService _marketplace;

        public MarketplaceController(IMarketplaceService marketplace)
        {
            _marketplace = marketplace;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProject(ListRequest request)
        {
            var result = await _marketplace.GetAllMarketplaceProject(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMarketplaceProject([FromForm] MarketplaceProjectAddRequest request)
        {
            var response = await _marketplace.CreateMarketplaceProject(request);
            return new ObjectResult(response)
            {
                StatusCode = response._statusCode
            };
        }
    }
}
