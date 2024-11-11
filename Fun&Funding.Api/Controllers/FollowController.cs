using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.FollowDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/follow")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }
        [HttpGet]
        public async Task<IActionResult> GetListFollower(Guid id)
        {
            var result = await _followService.GetListFollower(id);
            if (!result._isSuccess)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpPost("{id}/user")]
        public async Task<IActionResult> FollowUser(Guid id)
        {
            var result = await _followService.FollowUser(id);
            if (!result._isSuccess)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpPost("{id}/funding-project")]
        public async Task<IActionResult> FollowFundingProject(Guid id)
        {
            var result = await _followService.FollowProject(id);
            if (!result._isSuccess)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
}
