using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.Authentication;
using Fun_Funding.Application.ViewModel.AuthenticationDTO;
using Fun_Funding.Application.ViewModel.EmailDTO;
using Fun_Funding.Domain.Constrain;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Fun_Funding.Application.ViewModel.AuthenticationDTO.LoginRequest;
using RegisterRequest = Fun_Funding.Application.ViewModel.Authentication.RegisterRequest;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly Application.IService.IAuthenticationService _authService;

        public AuthenticationController(Application.IService.IAuthenticationService authService)
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
        [HttpGet("signin-google")]
        public IActionResult SignInGoogle(string? registeredRole = null)
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse), "Authentication");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl,
                Items = { 
                    { "prompt", "consent" },
                }
            };

            if (!string.IsNullOrEmpty(registeredRole))
                properties.Items.Add("registeredRole", registeredRole.ToString());

            if (registeredRole == null)
            {
                return Redirect("http://localhost:5173/choose-project-plan");
            }

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
                return Unauthorized();
            var claims = authenticateResult.Principal;
            var email = claims.FindFirst(ClaimTypes.Email)?.Value;
            var fullName = claims.FindFirst(ClaimTypes.Name)?.Value;
            var avatarUrl = claims.FindFirst(c => c.Type == "User_avatar")?.Value;

            //if (authenticateResult.Properties.Items.TryGetValue("isRegistered", out var customParam))
            //{
            //    Console.WriteLine($"Custom Parameter: {customParam}");
            //}

            var registeredRole = authenticateResult.Properties.Items.FirstOrDefault(i => i.Key == "registeredRole").Value;

            var loginResult = await _authService.LoginWithGoogle(email, fullName, avatarUrl, registeredRole);

            return Ok(loginResult);
        }

        [HttpGet("check-exist/{email}")]
        public async Task<IActionResult> CheckUserExistByEmail(string email)
        {
            var checkResult = await _authService.CheckUserExistByEmail(email);

            return Ok(checkResult);
        }


}
}
