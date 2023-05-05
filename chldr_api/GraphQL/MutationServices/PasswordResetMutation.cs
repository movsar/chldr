using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_shared.Models;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace chldr_api.GraphQL.MutationServices
{
    public class PasswordResetMutation : MutationService
    {
        internal async Task<PasswordResetResponse> ExecuteAsync(IConfiguration configuration, IStringLocalizer<AppLocalizations> localizer, EmailService emailService, string email)
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
                            localizer["Email:Reset_password_subject"],
                            localizer["Email:Reset_password_html", resetUrl]);

            emailService.Send(message);

            return new PasswordResetResponse() { Success = true, ResetToken = tokenValue };
        }
    }
}
