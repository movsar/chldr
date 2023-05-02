using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_shared.Models;
using chldr_tools;
using chldr_utils.Services;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
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
            var tokenExpiresAt = TimeSpan.FromMinutes(60);
            var tokenValue = JwtUtils.GenerateToken(user.UserId, "password-reset-secret", tokenExpiresAt);

            // Store the token in the Tokens table
            var token = new SqlToken
            {
                UserId = user.UserId,
                Type = (int)TokenType.PasswordReset,
                Value = tokenValue,
                ExpiresIn = DateTime.UtcNow.AddMinutes(60),
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
        public async Task<MutationResponse> ResetPassword([Service] SqlContext dbContext, string tokenValue, string newPassword)
        {
            var token = await dbContext.Tokens.FirstOrDefaultAsync(t => t.Type == (int)TokenType.PasswordReset && t.Value == tokenValue && t.ExpiresIn > DateTimeOffset.UtcNow);

            if (token == null)
            {
                throw new Exception("Invalid token");
            }

            var user = await dbContext.Users.FindAsync(token.UserId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Hash the new password and update the user's password in the Users table
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await dbContext.SaveChangesAsync();

            // Remove the used password reset token from the Tokens table
            dbContext.Tokens.Remove(token);
            await dbContext.SaveChangesAsync();

            return new MutationResponse() { Success = true };
        }

        [UseDbContext(typeof(SqlContext))]
        public async Task<SqlUser> RegisterUserAsync(string email, string password, string? firstName, string? lastName, string? patronymic, [ScopedService] SqlContext dbContext)
        {
            // Check if a user with this email already exists
            var existingUser = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                throw new GraphQLException("A user with this email already exists");
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

            return user;
        }

        [UseDbContext(typeof(SqlContext))]
        public async Task<SqlUser> LoginUserAsync(string email, string password, [ScopedService] SqlContext dbContext)
        {
            // Check if a user with this email exists
            var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new GraphQLException("No user found with this email");
            }

            // Check if the password is correct
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new GraphQLException("Incorrect password");
            }

            // Generate a new access token and calculate expiration time
            var secret = "asdf";
            var accessToken = JwtUtils.GenerateAccessToken(user.UserId, secret);
            var accessTokenExpiration = DateTime.UtcNow.AddSeconds(120);

            // Save the access token to the database
            dbContext.Tokens.Add(new SqlToken
            {
                UserId = user.UserId,
                Type = (int)TokenType.Access,
                Value = accessToken,
                ExpiresIn = accessTokenExpiration
            });
            await dbContext.SaveChangesAsync();
            return user;
        }
    }
}