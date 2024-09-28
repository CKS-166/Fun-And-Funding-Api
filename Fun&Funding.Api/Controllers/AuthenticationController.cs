using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.Authentication;
using Fun_Funding.Application.ViewModel.AuthenticationDTO;
using Fun_Funding.Application.ViewModel.EmailDTO;
using Fun_Funding.Domain.Constrain;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Fun_Funding.Application.ViewModel.AuthenticationDTO.LoginRequest;
using RegisterRequest = Fun_Funding.Application.ViewModel.Authentication.RegisterRequest;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> login(LoginRequest loginDTO)
        {
            var result = await _authService.LoginAsync(loginDTO);
            return Ok(result);
        }
        [HttpPost("register-backer")]
        public async Task<ActionResult<string>> RegisterBacker(RegisterRequest registerModel)
        {
            var result = await _authService.RegisterUserAsync(registerModel, new List<string> { Role.Backer });
            return Ok(result);
        }

        [HttpPost("register-admin")]
        public async Task<ActionResult<string>> RegisterAdmin(RegisterRequest registerModel)
        {
            var result = await _authService.RegisterUserAsync(registerModel, new List<string> { Role.Admin });
            return Ok(result);
        }

        [HttpPost("register-game-owner")]
        public async Task<ActionResult<string>> RegisterProjectOwner(RegisterRequest registerModel)
        {
            var result = await _authService.RegisterUserAsync(registerModel, new List<string> { Role.GameOwner });
            return Ok(result);
        }
        [HttpPost("password")]
        public async Task<IActionResult> SendResetPasswordEmailAsync([FromQuery] EmailRequest emailRequest)
        {
            var result = await _authService.SendResetPasswordEmailAsync(emailRequest);
            return Ok(result);
        }
        [HttpPatch("password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] NewPasswordRequest newPasswordRequest)
        {
            var result = await _authService.ResetPasswordAsync(newPasswordRequest);
            return BadRequest(result);
        }
    }
}
