﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
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
    public class RegisterUserResolver
    {
        internal async Task<RequestResult> ExecuteAsync(
            SqlDataProvider dataProvider,
            IStringLocalizer<AppLocalizations> _localizer,
            EmailService _emailService,
            string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            var unitOfWork = (SqlUnitOfWork)dataProvider.CreateUnitOfWork();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;
         
            unitOfWork.BeginTransaction();

            // Check if a user with this email already exists
            var existingUser = await usersRepository.FindByEmail(email);
            if (existingUser != null)
            {
                return new RequestResult() { ErrorMessage = "A user with this email already exists" };
            }

            // Create the new user
            var user = new UserDto
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Status = UserStatus.Unconfirmed,
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic
            };
            unitOfWork.Users.Add(user);

            var confirmationTokenExpiration = DateTime.UtcNow.AddDays(30);
            var confirmationToken = JwtService.GenerateToken(user.UserId, "confirmation-token-secretconfirmation-token-secretconfirmation-token-secret", confirmationTokenExpiration);

            // Save the tokens to the database
            unitOfWork.Tokens.Add(new TokenDto
            {
                UserId = user.UserId,
                Type = (int)TokenType.Confirmation,
                Value = confirmationToken,
                ExpiresIn = confirmationTokenExpiration
            });
            
            unitOfWork.Commit();

            var confirmEmailLink = new Uri(QueryHelpers.AddQueryString($"{AppConstants.Host}/login", new Dictionary<string, string?>(){
                { "token", confirmationToken},
            })).ToString();

            var message = new EmailMessage(new string[] { email },
                _localizer["Email:Confirm_email_subject"],
                _localizer["Email:Confirm_email_html", confirmEmailLink]);

            try
            {
                _emailService.Send(message);
                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(confirmationToken)
                };
            }
            catch (Exception ex)
            {
                //LogError(ex);
                return new RequestResult();
            }
        }
    }
}
