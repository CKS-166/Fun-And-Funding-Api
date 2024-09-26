using Fun_Funding.Application.IService;
using Fun_Funding.Application.IStorageService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.UserDTO;
using Fun_Funding.Domain.Constrain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IAzureService _storageService;
        public UserController(IAzureService storageService
            , IUserService userService)
        {
            _storageService = storageService;
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] ListRequest request)
        {
            var response = await _userService.GetUsers(request);
            return Ok(response);
        }
        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var response = await _userService.GetUserInfo();
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById([FromRoute] Guid id)
        {
            var response = await _userService.GetUserInfoById(id);
            return Ok(response);
        }
        [HttpPatch("info")]
        //[Authorize]
        public async Task<ActionResult> UpdateUser(UserUpdateRequest userUpdateRequest)
        {
            var response = await _userService.UpdateUser(userUpdateRequest);
            return Ok(response);
        }
        [HttpPatch("status/{id}")]
        //[Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> ChangeUserStatus([FromRoute] Guid id)
        {
            var response = await _userService.ChangeUserStatus(id);
            return Ok(response);
        }
    }
}
