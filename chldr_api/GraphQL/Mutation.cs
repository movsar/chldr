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
        private readonly SqlContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly EmailService _emailService;

        public Mutation(SqlContext dbContext, IConfiguration configuration,
            IStringLocalizer<AppLocalizations> localizer, EmailService emailService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _localizer = localizer;
            _emailService = emailService;
        }

        public async Task<PasswordResetResponse> PasswordReset(string email)
        {
            var passwordResetMutation = new PasswordResetMutation();
            return await passwordResetMutation.ExecuteAsync(_configuration, _dbContext, _localizer, _emailService, email);
        }

        public async Task<MutationResponse> UpdatePasswordAsync(string token, string newPassword)
        {
            var updatePasswordMutation = new UpdatePasswordMutation();
            return await updatePasswordMutation.ExecuteAsync(_dbContext, token, newPassword);
        }

        public async Task<MutationResponse> RegisterUserAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            var registerUserMutation = new RegisterUserMutation();
            return await registerUserMutation.ExecuteAsync(_dbContext, email, password, firstName, lastName, patronymic);
        }

        public async Task<LoginResponse> LoginUserAsync(string email, string password)
        {
            var loginUserMutation = new LoginUserMutation();
            return await loginUserMutation.ExecuteAsync(_dbContext, email, password);
        }
    }

}