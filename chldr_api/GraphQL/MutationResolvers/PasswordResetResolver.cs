using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.local.Services;
using chldr_data.Models;
using chldr_data.remote.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Resources.Localizations;
using chldr_data.Services;
using chldr_shared.Models;
using chldr_tools;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace chldr_api.GraphQL.MutationServices
{
    public class PasswordResetResolver
    {
        internal async Task<RequestResult> ExecuteAsync(
            SqlDataProvider dataProvider,
            IStringLocalizer<AppLocalizations> _localizer,
            EmailService _emailService,
            string email)
        {
            var unitOfWork = (SqlUnitOfWork)dataProvider.CreateUnitOfWork();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;
            var tokensRepository = (SqlTokensRepository)unitOfWork.Tokens;

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

            unitOfWork.Tokens.Add(token);
            
            // Send the password reset link to the user's email
            var resetPasswordLink = new Uri(QueryHelpers.AddQueryString($"{AppConstants.Host}/set-new-password", new Dictionary<string, string?>(){
                { "token", tokenValue }
            })).ToString();

            var emailBody = $"To reset your password, click the following link: {resetPasswordLink}";

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
    }
}
