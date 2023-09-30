using chldr_data;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Resources.Localizations;
using chldr_data.Services;
using chldr_shared.Models;
using chldr_tools;
using chldr_utils;
using chldr_utils.Services;
using GraphQLParser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Realms.Sync;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static HotChocolate.ErrorCodes;

namespace chldr_api.GraphQL.MutationServices
{
    public class UserResolver
    {
        private readonly IDataProvider _dataProvider;
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly EmailService _emailService;
        private readonly FileService _fileService;
        private readonly UserManager<SqlUser> _userManager;
        private readonly SignInManager<SqlUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly string _signingSecret;

        public UserResolver(
            IDataProvider dataProvider,
            IStringLocalizer<AppLocalizations> localizer,
            EmailService emailService,
            ExceptionHandler exceptionHandler,
            FileService fileService,
            IConfiguration configuration,
            UserManager<SqlUser> userManager,
            SignInManager<SqlUser> signInManager
            )
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

        internal async Task<RequestResult> SetNewPassword(string email, string token, string newPassword)
        {
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();

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
        internal async Task<RequestResult> ResetPassword(string email)
        {
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
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
        internal async Task<RequestResult> RefreshTokens(string accessToken, string refreshToken)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingSecret));

            var principal = GetPrincipalFromAccessToken(accessToken);
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

            // Generate new tokens
            var newAccessToken = JwtService.GenerateSignedToken(userId, _signingSecret);
            var newRefreshToken = JwtService.GenerateRefreshToken();

            await _userManager.RemoveAuthenticationTokenAsync(user, "RefreshTokenProvider", "RefreshToken");
            await _userManager.SetAuthenticationTokenAsync(user, "RefreshTokenProvider", "RefreshToken", newRefreshToken);

            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;
            var userModel = await usersRepository.GetAsync(user.Id);

            return new RequestResult
            {
                Success = true,
                SerializedData = JsonConvert.SerializeObject(
                    new
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken,
                        User = UserDto.FromModel(userModel)
                    })
            };
        }
        internal async Task<RequestResult> SignInAsync(string email, string password)
        {
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
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
        internal async Task<RequestResult> ConfirmEmailAsync(string token)
        {
            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();

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

                return new RequestResult() { Success = true };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return new RequestResult() { ErrorMessage = ex.Message };
            }
        }
        internal async Task<RequestResult> RegisterAndLogInAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;
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
                };

                if (string.IsNullOrEmpty(password))
                {
                    throw new NullReferenceException("Password is empty");
                }

                // Create the user
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                // Send email confirmation link
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmEmailLink = new Uri(QueryHelpers.AddQueryString($"{Constants.ProdFrontHost}/login",
                    new Dictionary<string, string?>() { { "token", confirmationToken }, })).ToString();

                var message = new EmailMessage(new string[] { email! },
                    _localizer["Email:Confirm_email_subject"],
                    _localizer["Email:Confirm_email_html", confirmEmailLink]);

                try
                {
                    _emailService.Send(message);

                    // Write the token to the user object
                    user.EmailConfirmationToken = confirmationToken;
                    await _userManager.UpdateAsync(user);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                unitOfWork.Commit();
                
                await _signInManager.SignInAsync(user, isPersistent: false);

                return await GenerateSignInResponse(user);
                
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                try
                {
                    var user = await usersRepository.GetByEmailAsync(email);
                    await usersRepository.RemoveAsync(user.Id);
                    unitOfWork.Commit();
                }
                catch (Exception) { }

                return new RequestResult() { ErrorMessage = ex.Message };
            }
        }
        private async Task<RequestResult> GenerateSignInResponse(SqlUser user)
        {
            // Generate JWT tokens
            var signingKeyAsText = _configuration.GetValue<string>("ApiJwtSigningKey");
            var accessToken = JwtService.GenerateSignedToken(user.Id, signingKeyAsText);
            var refreshToken = JwtService.GenerateRefreshToken();
            await _userManager.SetAuthenticationTokenAsync(user, "RefreshTokenProvider", "RefreshToken", refreshToken);

            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
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
        private ClaimsPrincipal GetPrincipalFromAccessToken(string token)
        {
            var signingSecret = _configuration.GetValue<string>("ApiJwtSigningKey");
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingSecret));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidIssuer = "Dosham",
                ValidAudience = "dosham.app",
                IssuerSigningKey = signingKey
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
