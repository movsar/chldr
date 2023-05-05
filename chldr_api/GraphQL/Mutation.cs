using chldr_api.GraphQL.MutationServices;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.Extensions.Localization;

namespace chldr_api
{
    public class Mutation
    {
        [UseDbContext(typeof(SqlContext))]
        public async Task<InitiatePasswordResetResponse> PasswordReset(
          [Service] IConfiguration configuration,
          [Service] SqlContext dbContext,
          [Service] IStringLocalizer<AppLocalizations> _localizer,
          [Service] EmailService emailService,
          string email)
        {
            return await PasswordResetMutation.ExecuteAsync(configuration, dbContext, _localizer, emailService, email);
        }

        [UseDbContext(typeof(SqlContext))]
        public async Task<MutationResponse> UpdatePasswordAsync(
            [Service] SqlContext dbContext,
            string token,
            string newPassword)
        {
            return await UpdatePasswordMutation.ExecuteAsync(dbContext, token, newPassword);
        }

        [UseDbContext(typeof(SqlContext))]
        public async Task<MutationResponse> RegisterUserAsync(
            [ScopedService] SqlContext dbContext,
            string email,
            string password,
            string? firstName,
            string? lastName,
            string? patronymic)
        {
            return await RegisterUserMutation.ExecuteAsync(dbContext, email, password, firstName, lastName, patronymic);
        }

        [UseDbContext(typeof(SqlContext))]
        public async Task<LoginResponse> LoginUserAsync(
            [ScopedService] SqlContext dbContext,
            string email,
            string password)
        {
            return await LoginUserMutation.ExecuteAsync(dbContext, email, password);
        }
    }
}