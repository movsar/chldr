﻿using chldr_api.GraphQL.MutationServices;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.Extensions.Localization;
using System.Configuration;

namespace chldr_api
{
    public class Mutation
    {
        private readonly PasswordResetResolver _passwordResetMutation;
        private readonly UpdatePasswordResolver _updatePasswordMutation;
        private readonly RegisterUserResolver _registerUserMutation;
        private readonly ConfirmEmailResolver _confirmEmailResolver;
        private readonly LoginResolver _loginUserMutation;

        protected readonly SqlContext _dbContext;
        protected readonly IConfiguration _configuration;
        protected readonly IStringLocalizer<AppLocalizations> _localizer;
        protected readonly EmailService _emailService;

        public Mutation(
            PasswordResetResolver passwordResetResolver,
            UpdatePasswordResolver updatePasswordResolver,
            RegisterUserResolver registerUserResolver,
            ConfirmEmailResolver confirmEmailResolver,
            LoginResolver loginUserResolver,
            IConfiguration configuration,
            IStringLocalizer<AppLocalizations> localizer,
            EmailService emailService)
        {
            _passwordResetMutation = passwordResetResolver;
            _updatePasswordMutation = updatePasswordResolver;
            _registerUserMutation = registerUserResolver;
            _confirmEmailResolver = confirmEmailResolver;
            _loginUserMutation = loginUserResolver;

            _dbContext = new SqlContext();
            _configuration = configuration;
            _localizer = localizer;
            _emailService = emailService;
        }

        public async Task<RegistrationResponse> RegisterUserAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            return await _registerUserMutation.ExecuteAsync(_dbContext, _localizer, _emailService, email, password, firstName, lastName, patronymic);
        }

        public async Task<MutationResponse> ConfirmEmailAsync(string token)
        {
            return await _confirmEmailResolver.ExecuteAsync(_dbContext, token);
        }

        public async Task<PasswordResetResponse> PasswordReset(string email)
        {
            return await _passwordResetMutation.ExecuteAsync(_dbContext, _configuration, _localizer, _emailService, email);
        }

        public async Task<MutationResponse> UpdatePasswordAsync(string token, string newPassword)
        {
            return await _updatePasswordMutation.ExecuteAsync(_dbContext, token, newPassword);
        }

        public async Task<LoginResponse> AutoLoginAsync(string refreshToken)
        {
            return await _loginUserMutation.ExecuteAsync(_dbContext, refreshToken);
        }

        public async Task<LoginResponse> LoginEmailPasswordAsync(string email, string password)
        {
            return await _loginUserMutation.ExecuteAsync(_dbContext, email, password);
        }
    }

}