using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace chldr_api.GraphQL.MutationServices
{
    public class LoginResolver
    {
        internal static async Task<LoginResponse> SignInAsync(SqlContext dbContext, SqlUser user)
        {
            // Generate a new access token and calculate expiration time
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(60);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(60);

            var accessToken = JwtService.GenerateToken(user.UserId, "access-token-secret", accessTokenExpiration);
            var refreshToken = JwtService.GenerateToken(user.UserId, "refresh-token-secret", refreshTokenExpiration);

            // Save the tokens to the database
            dbContext.Tokens.Add(new SqlToken
            {
                UserId = user.UserId,
                Type = (int)TokenType.Access,
                Value = accessToken,
                ExpiresIn = accessTokenExpiration
            });

            dbContext.Tokens.Add(new SqlToken
            {
                UserId = user.UserId,
                Type = (int)TokenType.Refresh,
                Value = refreshToken,
                ExpiresIn = refreshTokenExpiration
            });

            await dbContext.SaveChangesAsync();

            return new LoginResponse()
            {
                User = new UserDto(user),
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresIn = accessTokenExpiration,
                Success = true
            };
        }

        internal async Task<LoginResponse> ExecuteAsync(SqlContext dbContext, string refreshToken)
        {

            // Check if a user with this email exists
            var token = await dbContext.Tokens.SingleOrDefaultAsync(t => t.Type == (int)TokenType.Refresh && t.Value == refreshToken);
            if (token == null)
            {
                return new LoginResponse() { ErrorMessage = "Invalid refresh token" };
            }

            if (DateTime.UtcNow > token.ExpiresIn)
            {
                return new LoginResponse() { ErrorMessage = "Refresh token has expired" };
            }

            var user = dbContext.Users.SingleOrDefault(u => u.UserId.Equals(token.UserId));
            if (user == null)
            {
                return new LoginResponse() { ErrorMessage = "No user has been found for the requested token" };
            }

            // Remove previous tokens related to this user (in future this can be done in a batch job to increase efficiency)
            var previousTokens = dbContext.Tokens
                .Where(t => t.Type == (int)TokenType.Refresh || t.Type == (int)TokenType.Access)
                .Where(t => t.UserId.Equals(token.UserId));
            dbContext.Tokens.RemoveRange(previousTokens);

            return await SignInAsync(dbContext, user);
        }

        internal async Task<LoginResponse> ExecuteAsync(SqlContext dbContext, string email, string password)
        {
            // Check if a user with this email exists
            var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new LoginResponse() { ErrorMessage = "No user found with this email" };
            }

            // Check if the password is correct
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return new LoginResponse() { ErrorMessage = "Incorrect Password" };
            }

            return await SignInAsync(dbContext, user);
        }
    }
}
