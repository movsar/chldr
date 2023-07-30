using chldr_data;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.remote.Repositories;
using chldr_data.remote.Services;
using chldr_data.Resources.Localizations;
using chldr_data.Services;
using chldr_shared.Models;
using chldr_tools;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace chldr_api.GraphQL.MutationServices
{
    public class UserResolver
    {
        private readonly IDataProvider _dataProvider;
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly EmailService _emailService;
        private readonly FileService _fileService;

        public UserResolver(
            IDataProvider dataProvider,
            IStringLocalizer<AppLocalizations> localizer,
            EmailService emailService,
            ExceptionHandler exceptionHandler,
            FileService fileService)
        {
            _dataProvider = dataProvider;
            _localizer = localizer;
            _exceptionHandler = exceptionHandler;
            _emailService = emailService;
            _fileService = fileService;
        }
        internal async Task<RequestResult> Register(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;

            unitOfWork.BeginTransaction();

            try
            {
                // Check if a user with this email already exists
                var existingUser = await usersRepository.FindByEmail(email);
                if (existingUser != null)
                {
                    return new RequestResult() { ErrorMessage = "A user with this email already exists" };
                }

                // Create the new user
                var user = new UserDto
                {
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    Status = UserStatus.Unconfirmed,
                    FirstName = firstName,
                    LastName = lastName,
                    Patronymic = patronymic
                };
                await unitOfWork.Users.Add(user);

                var confirmationTokenExpiration = DateTime.UtcNow.AddDays(30);
                var confirmationToken = JwtService.GenerateToken(user.UserId, "confirmation-token-secretconfirmation-token-secretconfirmation-token-secret", confirmationTokenExpiration);

                // Save the tokens to the database
                await unitOfWork.Tokens.Add(new TokenDto
                {
                    UserId = user.UserId,
                    Type = (int)TokenType.Confirmation,
                    Value = confirmationToken,
                    ExpiresIn = confirmationTokenExpiration
                });


                var confirmEmailLink = new Uri(QueryHelpers.AddQueryString($"{Constants.ProdFrontHost}/login", new Dictionary<string, string?>(){
                { "token", confirmationToken},
            })).ToString();

                var message = new EmailMessage(new string[] { email },
                    _localizer["Email:Confirm_email_subject"],
                    _localizer["Email:Confirm_email_html", confirmEmailLink]);


                _emailService.Send(message);
                unitOfWork.Commit();

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(confirmationToken)
                };
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
            var tokenValue = JwtService.GenerateToken(user.UserId, "password-reset-secret", tokenExpiresIn);

            // Store the token in the Tokens table
            var token = new TokenDto
            {
                UserId = user.UserId,
                Type = (int)TokenType.PasswordReset,
                Value = tokenValue,
                ExpiresIn = tokenExpiresIn,
            };

            await unitOfWork.Tokens.Add(token);

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

            var accessToken = JwtService.GenerateToken(user.UserId, "access-token-secretaccess-token-secretaccess-token-secret", accessTokenExpiration);
            var refreshToken = JwtService.GenerateToken(user.UserId, "refresh-token-secretrefresh-token-secretrefresh-token-secret", refreshTokenExpiration);

            var accessTokenDto = new TokenDto
            {
                UserId = user.UserId,
                Type = (int)TokenType.Access,
                Value = accessToken,
                ExpiresIn = accessTokenExpiration
            };

            var refreshTokenDto = new TokenDto
            {
                UserId = user.UserId,
                Type = (int)TokenType.Refresh,
                Value = refreshToken,
                ExpiresIn = refreshTokenExpiration
            };

            // Save the tokens to the database
            await unitOfWork.Tokens.Add(accessTokenDto);
            await unitOfWork.Tokens.Add(refreshTokenDto);

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

        internal async Task<RequestResult> RefreshAccessCode(string refreshToken)
        {
            try
            {
                var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
                var usersRepository = (SqlUsersRepository)unitOfWork.Users;
                var tokensRepository = (SqlTokensRepository)unitOfWork.Tokens;

                unitOfWork.BeginTransaction();

                // Check if a user with this email exists
                var token = await tokensRepository.GetByValueAsync(refreshToken);
                if (token == null)
                {
                    return new RequestResult() { ErrorMessage = "Invalid refresh token" };
                }

                if (DateTime.UtcNow > token.ExpiresIn)
                {
                    return new RequestResult() { ErrorMessage = "Refresh token has expired" };
                }

                var user = await usersRepository.Get(token.UserId);
                if (user == null)
                {
                    return new RequestResult() { ErrorMessage = "No user has been found for the requested token" };
                }

                // Remove previous tokens related to this user (in future this can be done in a batch job to increase efficiency)
                var previousTokens = tokensRepository.GetByUserId(user.UserId);

                await tokensRepository.RemoveRange(previousTokens.Select(t => t.TokenId));

                return await SignInAsync(unitOfWork, user);
            }
            catch (Exception ex)
            {
                return new RequestResult()
                {
                    ErrorMessage = ex.Message
                };
            }
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

            var isVerified = await usersRepository.VerifyAsync(user.UserId, password);

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
                var tokensRepository = (SqlTokensRepository)unitOfWork.Tokens;

                var token = await tokensRepository.FindByValueAsync(tokenValue);
                if (token == null)
                {
                    return new RequestResult() { ErrorMessage = "Invalid token" };
                }

                var isExpired = JwtService.IsTokenExpired(token.Value);
                if (isExpired)
                {
                    return new RequestResult() { ErrorMessage = "Token has expired " };
                }

                var user = usersRepository.SetStatus(token.UserId, UserStatus.Active);
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
            var tokensRepository = (SqlTokensRepository)unitOfWork.Tokens;

            unitOfWork.BeginTransaction();

            var tokenInDatabase = await tokensRepository.GetPasswordResetTokenAsync(tokenValue);
            if (tokenInDatabase == null)
            {
                return new RequestResult("Invalid token");
            }

            var user = await unitOfWork.Users.Get(tokenInDatabase.UserId);
            if (user == null)
            {
                return new RequestResult("User not found");
            }

            var userDto = UserDto.FromModel(user);

            // Hash the new password and update the user's password in the Users table
            userDto.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await unitOfWork.Users.Update(userDto);

            // Remove the used password reset token from the Tokens table
            await unitOfWork.Tokens.Remove(tokenInDatabase.TokenId);

            unitOfWork.Commit();

            return new RequestResult() { Success = true };
        }
    }
}
