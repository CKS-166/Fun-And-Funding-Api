using Fun_Funding.Application.IService;
using Fun_Funding.Application.Services.EntityServices;
using Fun_Funding.Application.ViewModel.RequirementDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/requirement")]
    [ApiController]
    public class RequirementController : ControllerBase
    {
        private readonly IRequirementService _requirementService;

        public RequirementController(IRequirementService requirementService)
        {
            _requirementService = requirementService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateRequirement([FromBody] RequirementRequest request)
        {
            var result = await _requirementService.CreateNewRequirement(request);
            if (!result._isSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateRequirement([FromBody] UpdateRequirement request)
        {
            var result = await _requirementService.UpdateRequirement(request);
            if (!result._isSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
