using chldr_data;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using api_domain.Repositories;
using api_domain.Services;
using api_domain.SqlEntities;
using chldr_data.Resources.Localizations;
using chldr_data.Services;
using chldr_shared.Models;
using chldr_tools;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace chldr_api.GraphQL.MutationServices
{
    public class UserResolver
    {
        private readonly IDataProvider _dataProvider;
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly EmailService _emailService;
        private readonly IFileService _fileService;
        private readonly UserManager<SqlUser> _userManager;
        private readonly SignInManager<SqlUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly string _signingSecret;

        public UserResolver(
            IDataProvider dataProvider,
            IStringLocalizer<AppLocalizations> localizer,
            EmailService emailService,
            IExceptionHandler exceptionHandler,
            IFileService fileService,
            IConfiguration configuration,
            UserManager<SqlUser> userManager,
            SignInManager<SqlUser> signInManager)
        {
            _dataProvider = dataProvider;
            _localizer = localizer;
            _exceptionHandler = exceptionHandler;
            _emailService = emailService;
            _fileService = fileService;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _signingSecret = configuration.GetValue<string>("ApiJwtSigningKey")!;
        }

        public async Task<RequestResult> SetNewPassword(string email, string token, string newPassword)
        {
            var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();

            // Find the user
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new RequestResult()
                {
                    ErrorMessage = $"User with email {email} does not exist."
                };
            }

            // Reset the password
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                return new RequestResult()
                {
                    ErrorMessage = "Error resetting password: " + string.Join(", ", result.Errors.Select(x => x.Description))
                };
            }

            return new RequestResult()
            {
                Success = true
            };
        }
        public async Task<RequestResult> ResetPassword(string email)
        {
            var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email!.Equals(email));
            if (user == null)
            {
                throw new NullReferenceException("User not found");
            }

            // Generate a password reset token with a short expiration time
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Send the password reset link to the user's email
            var resetPasswordLink = new Uri(QueryHelpers.AddQueryString($"{Constants.ProdFrontHost}/set-new-password", new Dictionary<string, string?>(){
                { "email", user.Email},
                { "token", token }
            })).ToString();

            var message = new EmailMessage(new string[] { email },
                            _localizer["Email:Reset_password_subject"],
                            _localizer["Email:Reset_password_html", resetPasswordLink]);

            _emailService.Send(message);

            return new RequestResult()
            {
                Success = true,
                SerializedData = JsonConvert.SerializeObject(token)
            };
        }
        public async Task<RequestResult> RefreshTokens(string accessToken, string refreshToken)
        {
            var principal = JwtService.GetPrincipalFromAccessToken(accessToken, _signingSecret);
            var userId = principal.Identity.Name;

            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "RefreshTokenProvider", "RefreshToken");
            if (storedRefreshToken != refreshToken)
            {
                return new RequestResult()
                {
                    Success = false,
                    ErrorMessage = "Invalid token"
                };
            }

            await _userManager.RemoveAuthenticationTokenAsync(user, "RefreshTokenProvider", "RefreshToken");

            await _signInManager.SignInAsync(user, true);

            return await GenerateSignInResponse(user);
        }
        public async Task<RequestResult> SignInAsync(string email, string password)
        {
            var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;

            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email!.Equals(email));
                if (user == null)
                {
                    throw new NullReferenceException("User not found");
                }

                // Sign in the user
                var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    throw new Exception("Invalid credentials");
                }

                // Generate JWT tokens
                return await GenerateSignInResponse(user);
            }
            catch (Exception ex)
            {

                return new RequestResult()
                {
                    ErrorMessage = ex.Message
                };
            };
        }
        public async Task<RequestResult> ConfirmEmailAsync(string token)
        {
            using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();

            try
            {
                // Check if a user with this email already exists
                var usersRepository = (SqlUsersRepository)unitOfWork.Users;

                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("Token is required.");
                }

                var user = await _userManager.Users.SingleOrDefaultAsync(u => u.EmailConfirmationToken == token);
                if (user == null)
                {
                    throw new Exception("Invalid or expired token.");
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    throw new Exception("Error confirming email: " + result.Errors.First().Description);
                }

                // Activate user
                user.Status = (int)UserStatus.Active;
                user.Rate = UserModel.MemberRateRange.Lower;
                await _userManager.UpdateAsync(user);

                return new RequestResult() { Success = true };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw;
            }
        }
        public async Task<RequestResult> RegisterAndLogInAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();
            unitOfWork.BeginTransaction();

            try
            {
                var user = new SqlUser
                {
                    Email = email,
                    UserName = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Patronymic = patronymic,
                    Status = (int)UserStatus.Unconfirmed,
                    Rate = 0
                };

                if (string.IsNullOrEmpty(password))
                {
                    return new RequestResult() { ErrorMessage = "Password is empty" };
                }

                var existing = _userManager.Users.FirstOrDefault(u => u.Email.Equals(email));
                if (existing != null)
                {
                    return new RequestResult() { ErrorMessage = "User already exists" };
                }

                // Create the user
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    unitOfWork.Rollback();
                    return new RequestResult() { ErrorMessage = result.Errors.First().Description };
                }

                // Send email confirmation link
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmEmailLink = new Uri(QueryHelpers.AddQueryString($"{Constants.ProdApiHost}/user/confirm/",
                    new Dictionary<string, string?>() { { "token", confirmationToken }, })).ToString();

                var message = new EmailMessage(new string[] { email! },
                    _localizer["Email:Confirm_email_subject"],
                    _localizer["Email:Confirm_email_html", confirmEmailLink]);

                try
                {
                    _emailService.Send(message);
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    return new RequestResult() { ErrorMessage = "Error while sending the confirmation email" };
                }

                // Write the token to the user object
                user.EmailConfirmationToken = confirmationToken;
                await _userManager.UpdateAsync(user);

                unitOfWork.Commit();

                await _signInManager.SignInAsync(user, isPersistent: false);

                var response = await GenerateSignInResponse(user);
                return response;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                return new RequestResult() { ErrorMessage = ex.Message };
            }
        }
        private async Task<RequestResult> GenerateSignInResponse(SqlUser user)
        {
            // Generate JWT tokens
            var accessToken = JwtService.GenerateSignedToken(user.Id, _signingSecret);
            var refreshToken = JwtService.GenerateRefreshToken();
            await _userManager.SetAuthenticationTokenAsync(user, "RefreshTokenProvider", "RefreshToken", refreshToken);

            var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;
            var userModel = await usersRepository.GetByEmailAsync(user.Email);

            return new RequestResult
            {
                Success = true,
                SerializedData = JsonConvert.SerializeObject(new
                {
                    User = UserDto.FromModel(userModel),
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }),
            };
        }
    }
}
