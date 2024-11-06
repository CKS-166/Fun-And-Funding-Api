using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.MarketplaceProjectDTO;
using Fun_Funding.Domain.Constrain;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetAllProject([FromQuery] ListRequest request)
        {
            var result = await _marketplace.GetAllMarketplaceProject(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [Authorize(Roles = Role.GameOwner)]
        [HttpPost]
        public async Task<IActionResult> CreateMarketplaceProject([FromForm] MarketplaceProjectAddRequest request)
        {
            var response = await _marketplace.CreateMarketplaceProject(request);
            return new ObjectResult(response)
            {
                StatusCode = response._statusCode
            };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarketplaceProjectById([FromRoute] Guid id)
        {
            var response = await _marketplace.GetMarketplaceProjectById(id);
            return Ok(response);
        }

        [Authorize(Roles = Role.GameOwner)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarketplaceProject([FromRoute] Guid id)
        {
            await _marketplace.DeleteMarketplaceProject(id);
            return NoContent();
        }

        [Authorize(Roles = Role.GameOwner)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateMarketplaceProject([FromRoute] Guid id,
            [FromForm] MarketplaceProjectUpdateRequest request)
        {
            var response = await _marketplace.UpdateMarketplaceProject(id, request);
            return Ok(response);
        }
        [HttpPatch("{id}/approved")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> UpdateMarketplaceProjectStatus([FromRoute] Guid id,
            [FromForm] MarketplaceProjectUpdateRequest request)
        {
            var response = await _marketplace.UpdateMarketplaceProjectStatus(id, Domain.Enum.ProjectStatus.Approved);
            return Ok(response);
        }
        [HttpGet("top3")]
        public async Task<IActionResult> GetTop3MostFundedOngoingMarketplaceProject()
        {
            var response = await _marketplace.GetTop3MostFundedOngoingMarketplaceProject();
            return Ok(response);
        }
        [HttpGet("number-of-projects")]
        public async Task<IActionResult> CountPlatformProject()
        {
            var response = await _marketplace.CountPlatformProjects();
            return Ok(response);
        }
    }
}
