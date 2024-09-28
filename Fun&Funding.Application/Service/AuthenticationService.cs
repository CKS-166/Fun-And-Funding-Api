using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IEmailServices;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ITokenService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.Authentication;
using Fun_Funding.Application.ViewModel.AuthenticationDTO;
using Fun_Funding.Application.ViewModel.EmailDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private static Random random = new Random();
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthenticationService(IUnitOfWork unitOfWork,
            ITokenGenerator tokenGenerator,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IConfiguration configuration,
            IEmailService emailService
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<ResultDTO<string>> LoginAsync(LoginRequest loginDTO)
        {
            try
            {
                var getUser = await _unitOfWork.UserRepository.GetAsync(x => x.Email == loginDTO.Email);

                if (getUser is null || !await _userManager.CheckPasswordAsync(getUser, loginDTO.Password))
                    return ResultDTO<string>.Fail("Email or Password failed");
                //if (getUser.EmailConfirmed == false)
                //{
                //    var emailToken = await _userManager.GenerateTwoFactorTokenAsync(getUser, "Email");
                //    var mess = new Message(new string[] { getUser.Email! }, "OTP Verification", emailToken);
                //    _emailService.SendEmail(mess);
                //    return ResultDTO<ResponseToken>.Success(new ResponseToken { Token = $"OTP have been sent to your email {getUser.Email}" }, "otp_sent");
                //}

                var userRole = await _userManager.GetRolesAsync(getUser);
                var token = _tokenGenerator.GenerateToken(getUser, userRole);
                return ResultDTO<string>.Success(token, "token_generated");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<string>> RegisterUserAsync(RegisterRequest registerModel, IList<string> roles)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(registerModel.Email) || string.IsNullOrWhiteSpace(registerModel.Password))
                {
                    return ResultDTO<string>.Fail("Email and password are required.");
                }

                var existingUser = await _unitOfWork.UserRepository.GetAsync(x => x.Email == registerModel.Email);
                if (existingUser != null)
                {
                    return ResultDTO<string>.Fail("User already exists with this email.");
                }

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = registerModel.FullName,
                    UserName = registerModel.UserName,
                    Email = registerModel.Email,
                    CreatedDate = DateTime.Now,
                    UserStatus = UserStatus.Active,
                    NormalizedEmail = registerModel.Email.ToUpper(),
                    TwoFactorEnabled = true,
                };

                var result = await _userManager.CreateAsync(newUser, registerModel.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return ResultDTO<string>.Fail($"User creation failed: {errors}");
                }

                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    }
                    await _userManager.AddToRoleAsync(newUser, role);
                }

                var bankAccount = new BankAccount
                {
                    Id = Guid.NewGuid(),
                    BankCode = string.Empty,
                    BankNumber = string.Empty,
                    CreatedDate = DateTime.Now,
                };

                var wallet = new Wallet
                {
                    Id = Guid.NewGuid(),
                    Balance = 0,
                    Backer = newUser,
                    BankAccountId = bankAccount.Id,
                    CreatedDate = bankAccount.CreatedDate,
                };

                await _unitOfWork.BankAccountRepository.AddAsync(bankAccount);
                await _unitOfWork.WalletRepository.AddAsync(wallet);
                await _unitOfWork.CommitAsync();

                var token = _tokenGenerator.GenerateToken(newUser, roles);
                return ResultDTO<string>.Success(token, "Successfully created user and token");
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.Fail($"An error occurred: {ex.Message}");
            }
        }
        public async Task<ResultDTO<string>> SendResetPasswordEmailAsync(EmailRequest emailRequest)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetAsync(x => x.Email == emailRequest.ToEmail);
                if (user == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }
                else
                {
                    var logins = await _userManager.GetLoginsAsync(user);
                    var googleLogin = logins.FirstOrDefault(l => l.LoginProvider == "Google");
                    if(googleLogin != null)
                    {
                        throw new ExceptionError((int)HttpStatusCode.Forbidden, "Password reset is not available for accounts registered with Gmail. Please contact support for further assistance.");
                    }
                    else
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var frontendUrl = _configuration["FrontendUrl"];
                        var resetPasswordUrl = $"{frontendUrl}/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";
                        await _emailService.SendEmailAsync(emailRequest.ToEmail, "Password Recovery for Your Account", resetPasswordUrl, emailRequest.EmailType);
                        return ResultDTO<string>.Success("", $"An email containing instructions to reset your password has been sent to your registered email address: {emailRequest.ToEmail}.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is ExceptionError exceptionError)
                {
                    throw exceptionError;
                }
                throw new ExceptionError((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        public async Task<ResultDTO<string>> ResetPasswordAsync(NewPasswordRequest newPasswordRequest)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(newPasswordRequest.Email);
                if (user == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }

                var isTokenValid = await _userManager.VerifyUserTokenAsync(user, "Default", "ResetPassword", newPasswordRequest.Token);
                if (!isTokenValid)
                {
                    throw new ExceptionError((int)HttpStatusCode.Forbidden, "Invalid or expired token.");
                }

                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, newPasswordRequest.Token, newPasswordRequest.NewPassword);
                if (!resetPasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", resetPasswordResult.Errors.Select(e => e.Description));
                    throw new ExceptionError((int)HttpStatusCode.BadRequest, $"Failed to reset password: {errors}");
                }

                return ResultDTO<string>.Success("", "Your password has been reset successfully.");
            }
            catch (Exception ex)
            {
                if (ex is ExceptionError exceptionError)
                {
                    throw exceptionError;
                }
                throw new ExceptionError((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
