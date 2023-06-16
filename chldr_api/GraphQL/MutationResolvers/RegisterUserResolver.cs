using chldr_data.Enums;
using chldr_data.local.Services;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
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
    public class RegisterUserResolver
    {
        internal async Task<RegistrationResult> ExecuteAsync(
            SqlDataProvider dataProvider, 
            IStringLocalizer<AppLocalizations> _localizer, 
            EmailService _emailService,
            string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            var dbContext = dataProvider.GetContext();

            // Check if a user with this email already exists
            var existingUser = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                return new RegistrationResult() { ErrorMessage = "A user with this email already exists" };
            }

            // Create the new user
            var user = new SqlUser
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Status = (int)UserStatus.Unconfirmed,
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var confirmationTokenExpiration = DateTime.UtcNow.AddDays(30);
            var confirmationToken = JwtService.GenerateToken(user.UserId, "confirmation-token-secretconfirmation-token-secretconfirmation-token-secret", confirmationTokenExpiration);

            // Save the tokens to the database
            dbContext.Tokens.Add(new SqlToken
            {
                UserId = user.UserId,
                Type = (int)TokenType.Confirmation,
                Value = confirmationToken,
                ExpiresIn = confirmationTokenExpiration
            });
            await dbContext.SaveChangesAsync();

            var confirmEmailLink = new Uri(QueryHelpers.AddQueryString($"{AppConstants.Host}/login", new Dictionary<string, string?>(){
                { "token", confirmationToken},
            })).ToString();

            var message = new EmailMessage(new string[] { email },
                _localizer["Email:Confirm_email_subject"],
                _localizer["Email:Confirm_email_html", confirmEmailLink]);

            try
            {
                _emailService.Send(message);
                return new RegistrationResult()
                {
                    Success = true,
                    Token = confirmationToken
                };
            }
            catch (Exception ex)
            {
                //LogError(ex);
                return new RegistrationResult();
            }
        }
    }
}
