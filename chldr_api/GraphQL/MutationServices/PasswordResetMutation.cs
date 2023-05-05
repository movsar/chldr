using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_shared.Models;
using chldr_tools;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace chldr_api.GraphQL.MutationServices
{
    public class PasswordResetMutation : MutationService
    {
        public PasswordResetMutation(IConfiguration configuration, IStringLocalizer<AppLocalizations> localizer, EmailService emailService) : base(configuration, localizer, emailService) { }

        internal async Task<PasswordResetResponse> ExecuteAsync(string email)
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
            var host = _configuration.GetValue<string>("AppHost");

            var resetPasswordLink = new Uri(QueryHelpers.AddQueryString($"{AppConstants.Host}/set-new-password", new Dictionary<string, string?>(){
                { "token", tokenValue }
            })).ToString();

            var emailBody = $"To reset your password, click the following link: {resetPasswordLink}";

            var message = new EmailMessage(new string[] { email },
                            _localizer["Email:Reset_password_subject"],
                            _localizer["Email:Reset_password_html", resetPasswordLink]);

            _emailService.Send(message);

            return new PasswordResetResponse() { Success = true, ResetToken = tokenValue };
        }
    }
}
