using chldr_api.GraphQL.MutationServices;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Interfaces;
using chldr_data.local.Services;
using chldr_data.remote.Services;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_utils.Services;
using Microsoft.Extensions.Localization;

namespace chldr_api
{
    public class Mutation
    {
        private readonly PasswordResetResolver _passwordResetMutation;
        private readonly UpdatePasswordResolver _updatePasswordMutation;
        private readonly RegisterUserResolver _registerUserMutation;
        private readonly ConfirmEmailResolver _confirmEmailResolver;
        private readonly LoginResolver _loginUserMutation;
        private readonly IDataProvider _dataProvider;
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
            IDataProvider dataProvider,
            IConfiguration configuration,
            IStringLocalizer<AppLocalizations> localizer,
            EmailService emailService)
        {
            _passwordResetMutation = passwordResetResolver;
            _updatePasswordMutation = updatePasswordResolver;
            _registerUserMutation = registerUserResolver;
            _confirmEmailResolver = confirmEmailResolver;
            _loginUserMutation = loginUserResolver;

            _dataProvider = dataProvider;
            _configuration = configuration;
            _localizer = localizer;
            _emailService = emailService;

        }
        public async Task<InsertResponse> AddWord(string userId, EntryDto EntryDto)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            try
            {
                unitOfWork.Entries.Insert(EntryDto);
                unitOfWork.Commit();

                return new InsertResponse() { Success = true, CreatedAt = EntryDto.CreatedAt  };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw ex;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<MutationResponse> UpdateWord(string userId, EntryDto EntryDto)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            try
            {
                unitOfWork.Entries.Update(EntryDto);
                unitOfWork.Commit();

                return new MutationResponse() { Success = true };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw ex;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        // User mutations
        public async Task<RegistrationResponse> RegisterUserAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            return await _registerUserMutation.ExecuteAsync(_dataProvider as SqlDataProvider, _localizer, _emailService, email, password, firstName, lastName, patronymic);
        }

        public async Task<MutationResponse> ConfirmEmailAsync(string token)
        {
            return await _confirmEmailResolver.ExecuteAsync(_dataProvider as SqlDataProvider, token);
        }

        public async Task<PasswordResetResponse> PasswordReset(string email)
        {
            return await _passwordResetMutation.ExecuteAsync(_dataProvider as SqlDataProvider, _configuration, _localizer, _emailService, email);
        }

        public async Task<MutationResponse> UpdatePasswordAsync(string token, string newPassword)
        {
            return await _updatePasswordMutation.ExecuteAsync(_dataProvider as SqlDataProvider, token, newPassword);
        }

        public async Task<LoginResponse> LogInRefreshTokenAsync(string refreshToken)
        {
            return await _loginUserMutation.ExecuteAsync(_dataProvider as SqlDataProvider, refreshToken);
        }

        public async Task<LoginResponse> LoginEmailPasswordAsync(string email, string password)
        {
            return await _loginUserMutation.ExecuteAsync(_dataProvider as SqlDataProvider, email, password);
        }
    }

}