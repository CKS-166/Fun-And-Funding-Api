using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ITokenService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.Authentication;
using Fun_Funding.Application.ViewModel.AuthenticationDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public AuthenticationService(IUnitOfWork unitOfWork,
            ITokenGenerator tokenGenerator,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
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
                // Validate input
                if (string.IsNullOrWhiteSpace(registerModel.Email) || string.IsNullOrWhiteSpace(registerModel.Password))
                {
                    return ResultDTO<string>.Fail("Email and password are required.");
                }

                // Check if the user already exists
                var existingUser = await _unitOfWork.UserRepository.GetAsync(x => x.Email == registerModel.Email);
                if (existingUser != null)
                {
                    return ResultDTO<string>.Fail("User already exists with this email.");
                }

                // Create a new user
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = registerModel.FullName,
                    UserName = registerModel.UserName,
                    Email = registerModel.Email,
                    CreatedDate = DateTime.Now,
                    UserStatus = UserStatus.Active,
                    NormalizedEmail = registerModel.Email.ToUpper(),
                    TwoFactorEnabled = true, // Enable 2FA
                };

                // Add the user using UserManager
                var result = await _userManager.CreateAsync(newUser, registerModel.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return ResultDTO<string>.Fail($"User creation failed: {errors}");
                }

                // Assign roles
                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    }
                    await _userManager.AddToRoleAsync(newUser, role);
                }

                // Create a new Wallet for the user
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
                await _unitOfWork.CommitAsync(); // Commit all changes

                // Generate a token for the new user
                var token = _tokenGenerator.GenerateToken(newUser, roles);
                return ResultDTO<string>.Success(token, "Successfully created user and token");
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public static string GenerateRandomPassword(int length = 7)
        {
            try
            {
                const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
                const string numbers = "0123456789";
                const string specialChars = "!@#$%^&*()_-+=<>?";

                if (length < 7)
                {
                    throw new ExceptionError((int)HttpStatusCode.Forbidden, "Mật khẩu phải dài tối thiểu 7 kí tự");
                }

                var passwordChars = new StringBuilder();
                passwordChars.Append(upperCase[random.Next(upperCase.Length)]);
                passwordChars.Append(numbers[random.Next(numbers.Length)]);
                passwordChars.Append(specialChars[random.Next(specialChars.Length)]);

                string allChars = upperCase + lowerCase + numbers + specialChars;
                for (int i = passwordChars.Length; i < length; i++)
                {
                    passwordChars.Append(allChars[random.Next(allChars.Length)]);
                }

                return new string(passwordChars.ToString().OrderBy(c => random.Next()).ToArray());
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
