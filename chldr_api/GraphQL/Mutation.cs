using chldr_api.GraphQL.MutationServices;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.local.Services;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Realms;
using Realms.Sync;

namespace chldr_api
{
    public class Mutation
    {
        private readonly PasswordResetResolver _passwordResetMutation;
        private readonly UpdatePasswordResolver _updatePasswordMutation;
        private readonly RegisterUserResolver _registerUserMutation;
        private readonly ConfirmEmailResolver _confirmEmailResolver;
        private readonly LoginResolver _loginUserMutation;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly FileService _fileService;
        private readonly IDataProvider _dataProvider;

        protected readonly SqlContext _dbContext;
        protected readonly IStringLocalizer<AppLocalizations> _localizer;
        protected readonly EmailService _emailService;

        public Mutation(
            PasswordResetResolver passwordResetResolver,
            UpdatePasswordResolver updatePasswordResolver,
            RegisterUserResolver registerUserResolver,
            ConfirmEmailResolver confirmEmailResolver,
            LoginResolver loginUserResolver,

            IDataProvider dataProvider,
            IStringLocalizer<AppLocalizations> localizer,
            EmailService emailService,
            ExceptionHandler exceptionHandler,
            FileService fileService
            )
        {
            _passwordResetMutation = passwordResetResolver;
            _updatePasswordMutation = updatePasswordResolver;
            _registerUserMutation = registerUserResolver;
            _confirmEmailResolver = confirmEmailResolver;
            _loginUserMutation = loginUserResolver;
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
            _dataProvider = dataProvider;
            _localizer = localizer;
            _emailService = emailService;
        }

        public RequestResult AddEntry(string userId, EntryDto entryDto)
        {
            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            try
            {
                var changeSets = unitOfWork.EntryService.Add(userId, entryDto);
                unitOfWork.Commit();

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(new
                    {
                        ChangeSets = changeSets,
                        CreatedAt = entryDto.CreatedAt
                    })
                };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                _exceptionHandler.LogError(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return new RequestResult();
        }

        public RequestResult UpdateEntry(string userId, EntryDto entryDto)
        {
            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            try
            {
                var changeSets = unitOfWork.EntryService.Update(userId, entryDto);
                unitOfWork.Commit();

                return new RequestResult() { Success = true, SerializedData = JsonConvert.SerializeObject(changeSets) };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                _exceptionHandler.LogError(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return new RequestResult();
        }

        public RequestResult RemoveEntry(string userId, string entryId)
        {
            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            try
            {
                var changeSets = unitOfWork.EntryService.Remove(userId, entryId);
                unitOfWork.Commit();

                return new RequestResult() { Success = true, SerializedData = JsonConvert.SerializeObject(changeSets) };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                _exceptionHandler.LogError(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return new RequestResult();
        }

        // User mutations
        public async Task<RequestResult> RegisterUserAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            return await _registerUserMutation.ExecuteAsync((SqlDataProvider)_dataProvider, _localizer, _emailService, email, password, firstName, lastName, patronymic);
        }

        public async Task<RequestResult> ConfirmEmailAsync(string token)
        {
            return await _confirmEmailResolver.ExecuteAsync((SqlDataProvider)_dataProvider, token);
        }

        public async Task<RequestResult> PasswordReset(string email)
        {
            return await _passwordResetMutation.ExecuteAsync((SqlDataProvider)_dataProvider, _localizer, _emailService, email);
        }

        public async Task<RequestResult> UpdatePasswordAsync(string token, string newPassword)
        {
            return await _updatePasswordMutation.ExecuteAsync((SqlDataProvider)_dataProvider, token, newPassword);
        }

        public async Task<RequestResult> LogInRefreshTokenAsync(string refreshToken)
        {
            return await _loginUserMutation.ExecuteAsync((SqlDataProvider)_dataProvider, refreshToken);
        }

        public async Task<RequestResult> LoginEmailPasswordAsync(string email, string password)
        {
            return await _loginUserMutation.ExecuteAsync((SqlDataProvider)_dataProvider, email, password);
        }
    }

}