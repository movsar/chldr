﻿using chldr_data.Dto;
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
    public class LoginUserMutation : MutationService
    {
        public LoginUserMutation(IConfiguration configuration, IStringLocalizer<AppLocalizations> localizer, EmailService emailService) : base(configuration, localizer, emailService)        {        }

        internal async Task<LoginResponse> ExecuteAsync(string email, string password)
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

            // Generate a new access token and calculate expiration time
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(120);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(30);

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
    }
}
