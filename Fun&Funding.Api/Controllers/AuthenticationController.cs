using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.Authentication;
using Fun_Funding.Application.ViewModel.AuthenticationDTO;
using Fun_Funding.Domain.Constrain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<string>> registerBacker(RegisterRequest registerModel)
        {
            var result = await _authService.RegisterUserAsync(registerModel, Role.Backer);
            return Ok(result);
        }
        [HttpPost("register-admin")]
        public async Task<ActionResult<string>> registerAdmin(RegisterRequest registerModel)
        {
            var result = await _authService.RegisterUserAsync(registerModel, Role.Admin);
            return Ok(result);
        }
        [HttpPost("register-projectOwner")]
        public async Task<ActionResult<string>> registerProjectOwner(RegisterRequest registerModel)
        {
            var result = await _authService.RegisterUserAsync(registerModel, Role.GameOwner);
            return Ok(result);
        }
    }
}
