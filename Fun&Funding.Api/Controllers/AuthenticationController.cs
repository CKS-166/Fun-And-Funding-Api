using Fun_Funding.Application.IEmailService;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.Authentication;
using Fun_Funding.Application.ViewModel.AuthenticationDTO;
using Fun_Funding.Application.ViewModel.EmailDTO;
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
        private readonly IEmailService _emailService;

        public AuthenticationController(IAuthenticationService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
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
        [HttpPost("SendMail")]
        public async Task<IActionResult> SendMail([FromBody] EmailRequest emailRequest)
        {
            await _emailService.SendEmailAsync(emailRequest.ToEmail, emailRequest.Subject, emailRequest.Body);
            return Ok("Email sent successfully!");
        }
    }
}
