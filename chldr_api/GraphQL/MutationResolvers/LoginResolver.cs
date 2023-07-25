
using chldr_data.Enums;
using chldr_data.Resources.Localizations;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Interfaces;
using chldr_data.local.Services;
using Newtonsoft.Json;
using chldr_data.Models;
using chldr_data.Services;
using chldr_data.remote.Repositories;

namespace chldr_api.GraphQL.MutationServices
{
    public class LoginResolver
    {
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
            await unitOfWork.Tokens.Add(accessTokenDto, null);
            await unitOfWork.Tokens.Add(refreshTokenDto, null);

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

        internal async Task<RequestResult> ExecuteAsync(SqlDataProvider dataProvider, string refreshToken)
        {
            try
            {
                var unitOfWork = (SqlUnitOfWork)dataProvider.CreateUnitOfWork();
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

                await tokensRepository.RemoveRange(previousTokens.Select(t => t.TokenId), null);

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

        internal async Task<RequestResult> ExecuteAsync(SqlDataProvider dataProvider, string email, string password)
        {
            var unitOfWork = (SqlUnitOfWork)dataProvider.CreateUnitOfWork();
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
    }
}
