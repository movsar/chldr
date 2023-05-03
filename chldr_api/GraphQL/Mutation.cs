using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_shared.Models;
using chldr_tools;
using chldr_utils.Services;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Localization;
using System.Web;

namespace chldr_api
{
    public class Mutation
    {
        [UseDbContext(typeof(SqlContext))]
        public async Task<InitiatePasswordResetResponse> InitiatePasswordReset(
          [Service] IConfiguration configuration,
          [Service] SqlContext dbContext, [Service] IStringLocalizer<AppLocalizations> _localizer,
          [Service] EmailService emailService,
          string email)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new ArgumentException($"User with email {email} does not exist.");
            }

            // Generate a password reset token with a short expiration time
            var tokenExpiresIn = DateTime.UtcNow.AddMinutes(60);
            var tokenValue = JwtService.GenerateToken(user.UserId, "password-reset-secret", tokenExpiresIn);

            // Store the token in the Tokens table
            var token = new SqlToken
            {
                UserId = user.UserId,
                Type = (int)TokenType.PasswordReset,
                Value = tokenValue,
                ExpiresIn = tokenExpiresIn,
            };

            dbContext.Tokens.Add(token);
            await dbContext.SaveChangesAsync();

            // Send the password reset link to the user's email
            var host = configuration.GetValue<string>("AppHost");
            var resetUrl = @$"{host}/set-new-password?token={tokenValue}";
            var emailBody = $"To reset your password, click the following link: {resetUrl}";

            var message = new EmailMessage(new string[] { email },
                            _localizer["Email:Reset_password_subject"],
                            _localizer["Email:Reset_password_html", resetUrl]);

            emailService.Send(message);

            return new InitiatePasswordResetResponse() { Success = true, ResetToken = tokenValue };
        }

        [UseDbContext(typeof(SqlContext))]
        public async Task<MutationResponse> UpdatePasswordAsync([Service] SqlContext dbContext, string token, string newPassword)
        {
            var tokenInDatabase = await dbContext.Tokens.FirstOrDefaultAsync(t => t.Type == (int)TokenType.PasswordReset && t.Value == token && t.ExpiresIn > DateTimeOffset.UtcNow);

            if (tokenInDatabase == null)
            {
                return new MutationResponse("Invalid token");
            }

            var user = await dbContext.Users.FindAsync(tokenInDatabase.UserId);
            if (user == null)
            {
                return new MutationResponse("User not found");
            }

            // Hash the new password and update the user's password in the Users table
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await dbContext.SaveChangesAsync();

            // Remove the used password reset token from the Tokens table
            dbContext.Tokens.Remove(tokenInDatabase);
            await dbContext.SaveChangesAsync();

            return new MutationResponse() { Success = true };
        }

        [UseDbContext(typeof(SqlContext))]
        public async Task<MutationResponse> RegisterUserAsync(string email, string password, string? firstName, string? lastName, string? patronymic, [ScopedService] SqlContext dbContext)
        {
            // Check if a user with this email already exists
            var existingUser = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                return new MutationResponse("A user with this email already exists");
            }

            // Create the new user
            var user = new SqlUser
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            return new MutationResponse() { Success = true };
        }

        [UseDbContext(typeof(SqlContext))]
        public async Task<LoginResponse> LoginUserAsync(string email, string password, [ScopedService] SqlContext dbContext)
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