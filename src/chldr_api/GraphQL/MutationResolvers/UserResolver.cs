using chldr_data;
using chldr_data.DatabaseObjects.Dtos;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace chldr_api.GraphQL.MutationServices
{
    public class UserResolver
    {
        private readonly IDataProvider _dataProvider;
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly EmailService _emailService;
        private readonly FileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<SqlUser> _userManager;
        private readonly SignInManager<SqlUser> _signInManager;

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
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;

        }
        internal async Task<RequestResult> RegisterAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();

            unitOfWork.BeginTransaction();

            try
            {
                // Create the new user
                var user = new SqlUser
                {
                    Email = email,
                    UserName = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Patronymic = patronymic,
                };

                var confirmationTokenExpiration = DateTime.UtcNow.AddDays(30);
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmEmailLink = new Uri(QueryHelpers.AddQueryString($"{Constants.ProdFrontHost}/login", new Dictionary<string, string?>(){
                    { "token", confirmationToken},
                })).ToString();

                var message = new EmailMessage(new string[] { email },
                    _localizer["Email:Confirm_email_subject"],
                    _localizer["Email:Confirm_email_html", confirmEmailLink]);

                _emailService.Send(message);

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Generate JWT token
                    var tokenHandler = new JwtSecurityTokenHandler();

                    var signingSecret = _configuration.GetValue<string>("ApiJwtSigningKey");
                    var key = Encoding.UTF8.GetBytes(signingSecret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, user.Id.ToString())
                            // Add other claims as needed
                        }),
                        Expires = DateTime.UtcNow.AddDays(7), // Token expiration, adjust as needed
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    unitOfWork.Commit();

                    return new RequestResult { Success = true, SerializedData = JsonConvert.SerializeObject(tokenString) };
                }
                else
                {
                    return new RequestResult { Success = false, ErrorMessage = result.Errors.First().Description };
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return new RequestResult() { ErrorMessage = ex.Message };
            }
        }
        internal async Task<RequestResult> ResetPassword(string email)
        {
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;

            var user = await usersRepository.FindByEmail(email);
            if (user == null)
            {
                throw new ArgumentException($"User with email {email} does not exist.");
            }

            // Generate a password reset token with a short expiration time
            var tokenExpiresIn = DateTime.UtcNow.AddMinutes(60);
            var tokenValue = JwtService.GenerateToken(user.Id, "password-reset-secret", tokenExpiresIn);

            // Store the token in the Tokens table

            // Send the password reset link to the user's email
            var resetPasswordLink = new Uri(QueryHelpers.AddQueryString($"{Constants.ProdFrontHost}/set-new-password", new Dictionary<string, string?>(){
                { "token", tokenValue }
            })).ToString();

            var message = new EmailMessage(new string[] { email },
                            _localizer["Email:Reset_password_subject"],
                            _localizer["Email:Reset_password_html", resetPasswordLink]);

            _emailService.Send(message);

            return new RequestResult()
            {
                Success = true,
                SerializedData = JsonConvert.SerializeObject(tokenValue)
            };
        }
        internal static async Task<RequestResult> SignInAsync(SqlUnitOfWork unitOfWork, UserModel user)
        {
            // Generate a new access token and calculate expiration time
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(60);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(60);

            var accessToken = JwtService.GenerateToken(user.Id, "access-token-secretaccess-token-secretaccess-token-secret", accessTokenExpiration);
            var refreshToken = JwtService.GenerateToken(user.Id, "refresh-token-secretrefresh-token-secretrefresh-token-secret", refreshTokenExpiration);

            unitOfWork.Commit();

            return new RequestResult()
            {
                SerializedData = JsonConvert.SerializeObject(new
                {
                    User = UserDto.FromModel(user),
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiresIn = accessTokenExpiration,
                    Success = true
                })
            };
        }

        internal async Task<RequestResult> LogIn(string email, string password)
        {
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;

            unitOfWork.BeginTransaction();

            // Check if a user with this email exists
            var user = await usersRepository.FindByEmail(email);
            if (user == null)
            {
                return new RequestResult() { ErrorMessage = "No user found with this email" };
            }

            // Check if the password is correct

            var isVerified = await usersRepository.VerifyAsync(user.Id, password);

            if (!isVerified)
            {
                return new RequestResult() { ErrorMessage = "Incorrect Password" };
            }

            return await SignInAsync(unitOfWork, user);
        }
        internal async Task<RequestResult> Confirm(string tokenValue)
        {
            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
            unitOfWork.BeginTransaction();

            try
            {
                // Check if a user with this email already exists
                var usersRepository = (SqlUsersRepository)unitOfWork.Users;


                unitOfWork.Commit();
                return new RequestResult() { Success = true };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return new RequestResult() { ErrorMessage = ex.Message };
            }
        }
        internal async Task<RequestResult> UpdatePassword(string tokenValue, string newPassword)
        {
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;

            unitOfWork.BeginTransaction();


            unitOfWork.Commit();

            return new RequestResult() { Success = true };
        }
    }
}
