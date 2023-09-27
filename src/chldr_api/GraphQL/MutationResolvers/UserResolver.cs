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
        private readonly UserManager<SqlUser> _userManager;
        private readonly SignInManager<SqlUser> _signInManager;
        private readonly string _signingSecret;
        private readonly SymmetricSecurityKey _signingKey;

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

            _signingSecret = configuration.GetValue<string>("ApiJwtSigningKey")!;
        }
        internal async Task<RequestResult> RegisterAndLogInAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();

            unitOfWork.BeginTransaction();

            try
            {
                var usersRepository = (SqlUsersRepository)unitOfWork.Users;

                // Register
                await usersRepository.RegisterAsync(new UserDto()
                {
                    Email = email,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName,
                    Patronymic = patronymic,
                });

                // Sign In
                var accessToken = await usersRepository.SignInAsync(email, _signingSecret);

                unitOfWork.Commit();

                return new RequestResult { Success = true, SerializedData = JsonConvert.SerializeObject(accessToken) };

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

        internal async Task<RequestResult> LogIn(string email, string password)
        {
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;

            var accessToken = await usersRepository.SignInAsync(email, password, _signingSecret);

            return new RequestResult()
            {
                Success = true,
                SerializedData = JsonConvert.SerializeObject(accessToken)
            };
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
