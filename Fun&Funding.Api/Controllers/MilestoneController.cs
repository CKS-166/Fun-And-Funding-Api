using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.MilestoneDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/milestone")]
    [ApiController]
    public class MilestoneController : ControllerBase
    {
        private readonly IMilestoneService _milestoneService;

        public MilestoneController(IMilestoneService milestoneService)
        {
            _milestoneService = milestoneService;
        }
        [HttpGet("get-group-latest-milestone")]
        public async Task<IActionResult> GetAllMilestoneOrder()
        {
            var result = await _milestoneService.GetListLastestMilestone();
            if (!result._isSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("get-by-verson-or-order")]
        public async Task<IActionResult> GetMilestoneByVersionOrOrder([FromQuery] int? Order, int? Version)
        {
            var result = await _milestoneService.GetMilestoneByVersionAndOrder(Order,Version);
            if (!result._isSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewMilestone(AddMilestoneRequest request)
        {
            var result = await _milestoneService.CreateMilestone(request);
            if (!result._isSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
