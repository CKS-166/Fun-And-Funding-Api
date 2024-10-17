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
        [HttpPost]
        public async Task<IActionResult> FollowUser(FollowRequest request)
        {
            var result = await _followService.FollowUser(request);
            if (!result._isSuccess)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
}
