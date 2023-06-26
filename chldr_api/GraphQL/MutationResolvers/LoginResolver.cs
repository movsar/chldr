
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

namespace chldr_api.GraphQL.MutationServices
{
    public class LoginResolver
    {
        internal static async Task<RequestResult> SignInAsync(SqlContext dbContext, SqlUser user)
        {
            // Generate a new access token and calculate expiration time
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(60);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(60);

            var accessToken = JwtService.GenerateToken(user.UserId, "access-token-secretaccess-token-secretaccess-token-secret", accessTokenExpiration);
            var refreshToken = JwtService.GenerateToken(user.UserId, "refresh-token-secretrefresh-token-secretrefresh-token-secret", refreshTokenExpiration);

            var accessTokenEntity = new SqlToken
            {
                UserId = user.UserId,
                Type = (int)TokenType.Access,
                Value = accessToken,
                ExpiresIn = accessTokenExpiration
            };

            var refreshTokenEntity = new SqlToken
            {
                UserId = user.UserId,
                Type = (int)TokenType.Refresh,
                Value = refreshToken,
                ExpiresIn = refreshTokenExpiration
            };

            // Save the tokens to the database
            dbContext.Tokens.Add(accessTokenEntity);
            dbContext.Tokens.Add(refreshTokenEntity);

            await dbContext.SaveChangesAsync();

            return new RequestResult()
            {
                SerializedData = JsonConvert.SerializeObject(new
                {
                    User = UserDto.FromModel(UserModel.FromEntity(user)),
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
                var dbContext = dataProvider.GetContext();

                // Check if a user with this email exists
                var token = await dbContext.Tokens.SingleOrDefaultAsync(t => t.Type == (int)TokenType.Refresh && t.Value == refreshToken);
                if (token == null)
                {
                    return new RequestResult() { ErrorMessage = "Invalid refresh token" };
                }

                if (DateTime.UtcNow > token.ExpiresIn)
                {
                    return new RequestResult() { ErrorMessage = "Refresh token has expired" };
                }

                var user = dbContext.Users.SingleOrDefault(u => u.UserId.Equals(token.UserId));
                if (user == null)
                {
                    return new RequestResult() { ErrorMessage = "No user has been found for the requested token" };
                }

                // Remove previous tokens related to this user (in future this can be done in a batch job to increase efficiency)
                var previousTokens = dbContext.Tokens
                    .Where(t => t.Type == (int)TokenType.Refresh || t.Type == (int)TokenType.Access)
                    .Where(t => t.UserId.Equals(token.UserId));

                dbContext.Tokens.RemoveRange(previousTokens);

                return await SignInAsync(dbContext, user);
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
            var dbContext = dataProvider.GetContext();

            // Check if a user with this email exists
            var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new RequestResult() { ErrorMessage = "No user found with this email" };
            }

            // Check if the password is correct
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return new RequestResult() { ErrorMessage = "Incorrect Password" };
            }

            return await SignInAsync(dbContext, user);
        }
    }
}
