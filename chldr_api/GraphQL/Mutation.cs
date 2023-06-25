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
            using var unitOfWork = (ISqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            try
            {
                // Process added entry
                unitOfWork.Entries.Add(entryDto);

                var entryChangeSet = ChangeSetDto.Create(Operation.Insert, userId, RecordType.Entry, entryDto.EntryId);
                unitOfWork.ChangeSets.Add(entryChangeSet);
                
                // Process added translations
                foreach (var translation in entryDto.Translations)
                {
                    unitOfWork.Translations.Add(translation);

                    var translationChangeSet = ChangeSetDto.Create(Operation.Insert, userId, RecordType.Translation, translation.TranslationId);
                    unitOfWork.ChangeSets.Add(translationChangeSet);
                }

                // Process added sounds
                foreach (var sound in entryDto.Sounds)
                {
                    unitOfWork.Sounds.Add(sound);

                    var translationChangeSet = ChangeSetDto.Create(Operation.Insert, userId, RecordType.Sound, sound.SoundId);
                    unitOfWork.ChangeSets.Add(translationChangeSet);
                }

                unitOfWork.Commit();

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(new
                    {
                        entryDto.CreatedAt
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

            return new RequestResult() { Success = false };
        }

        public RequestResult UpdateEntry(string userId, EntryDto updatedEntryDto)
        {
            var resultingChangeSetDtos = new List<ChangeSetDto>();

            using var unitOfWork = (ISqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            try
            {
                var existingEntry = unitOfWork.Entries.Get(updatedEntryDto.EntryId);
                var existingEntryDto = EntryDto.FromModel(existingEntry);

                // Update entry
                var entryChanges = Change.GetChanges(updatedEntryDto, existingEntryDto);
                if (entryChanges.Count != 0)
                {
                    unitOfWork.Entries.Update(updatedEntryDto);

                    var entryChangeSetDto = ChangeSetDto.Create(Operation.Update, userId, RecordType.Entry, updatedEntryDto.EntryId, entryChanges);
                    resultingChangeSetDtos.Add(entryChangeSetDto);
                }

                // Add / Remove / Update translations
                var translationChangeSets = ProcessTranslationsForEntryUpdate(unitOfWork, userId, existingEntryDto, updatedEntryDto);
                resultingChangeSetDtos.AddRange(translationChangeSets);

                // Add / Remove / Update sounds
                var soundChangeSets = ProcessSoundsForEntryUpdate(unitOfWork, userId, existingEntryDto, updatedEntryDto);
                resultingChangeSetDtos.AddRange(soundChangeSets);

                // Insert changesets
                unitOfWork.ChangeSets.AddRange(resultingChangeSetDtos);

                unitOfWork.Commit();

                return new RequestResult() { Success = true };
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

            return new RequestResult() { Success = false };
        }

        private IEnumerable<ChangeSetDto> ProcessSoundsForEntryUpdate(ISqlUnitOfWork unitOfWork, string userId, EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            var changeSets = new List<ChangeSetDto>();

            var existingEntrySoundIds = existingEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();
            var updatedEntrySoundIds = updatedEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();

            var added = updatedEntryDto.Sounds.Where(t => !existingEntrySoundIds.Contains(t.SoundId));
            var deleted = existingEntryDto.Sounds.Where(t => !updatedEntrySoundIds.Contains(t.SoundId));
            var updated = updatedEntryDto.Sounds.Where(t => existingEntrySoundIds.Contains(t.SoundId) && updatedEntrySoundIds.Contains(t.SoundId));

            // Process inserted translations
            foreach (var sound in added)
            {
                unitOfWork.Sounds.Add(sound);

                var changeSet = ChangeSetDto.Create(Operation.Insert, userId, RecordType.Sound, sound.SoundId);
                changeSets.Add(changeSet);
            }

            // Process removed translations
            foreach (var sound in deleted)
            {
                unitOfWork.Sounds.Remove(sound.SoundId);

                var changeSet = ChangeSetDto.Create(Operation.Delete, userId, RecordType.Sound, sound.SoundId);
                changeSets.Add(changeSet);
            }

            // Process updated translations
            foreach (var sound in updated)
            {
                var existingDto = existingEntryDto.Sounds.First(t => t.SoundId.Equals(sound.SoundId));

                var changes = Change.GetChanges(sound, existingDto);
                if (changes.Count == 0)
                {
                    continue;
                }

                unitOfWork.Sounds.Update(sound);

                var changeSet = ChangeSetDto.Create(Operation.Update, userId, RecordType.Sound, sound.SoundId, changes);
                changeSets.Add(changeSet);
            }

            return changeSets;
        }
        private IEnumerable<ChangeSetDto> ProcessTranslationsForEntryUpdate(IUnitOfWork unitOfWork, string userId, EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            var changeSets = new List<ChangeSetDto>();

            var existingEntryTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedEntryTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var added = updatedEntryDto.Translations.Where(t => !existingEntryTranslationIds.Contains(t.TranslationId));
            var deleted = existingEntryDto.Translations.Where(t => !updatedEntryTranslationIds.Contains(t.TranslationId));
            var updated = updatedEntryDto.Translations.Where(t => existingEntryTranslationIds.Contains(t.TranslationId) && updatedEntryTranslationIds.Contains(t.TranslationId));

            // Process inserted translations
            foreach (var translation in added)
            {
                unitOfWork.Translations.Add(translation);

                var changeSet = ChangeSetDto.Create(Operation.Insert, userId, RecordType.Translation, translation.TranslationId);
                changeSets.Add(changeSet);
            }

            // Process removed translations
            foreach (var translation in deleted)
            {
                unitOfWork.Translations.Remove(translation.TranslationId);

                var changeSet = ChangeSetDto.Create(Operation.Delete, userId, RecordType.Translation, translation.TranslationId);
                changeSets.Add(changeSet);
            }

            // Process updated translations
            foreach (var translation in updated)
            {
                var existingTranslationDto = existingEntryDto.Translations.First(t => t.TranslationId.Equals(translation.TranslationId));

                var changes = Change.GetChanges(translation, existingTranslationDto);
                if (changes.Count == 0)
                {
                    continue;
                }

                unitOfWork.Translations.Update(translation);

                var changeSet = ChangeSetDto.Create(Operation.Update, userId, RecordType.Translation, translation.TranslationId, changes);
                changeSets.Add(changeSet);
            }

            return changeSets;
        }

        public RequestResult RemoveEntry(string userId, string entryId)
        {
            using var unitOfWork = (ISqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            try
            {
                var entry = unitOfWork.Entries.Get(entryId);
                var soundIds = entry.Sounds.Select(s => s.SoundId).ToArray();
                var translationIds = entry.Translations.Select(t => t.TranslationId).ToArray();

                // Process removed translations
                foreach (var translationId in translationIds)
                {
                    unitOfWork.Translations.Remove(translationId);

                    var translationChangeSet = ChangeSetDto.Create(Operation.Delete, userId, RecordType.Translation, translationId);
                    unitOfWork.ChangeSets.Add(translationChangeSet);
                }

                // Process removed sounds
                foreach (var soundId in soundIds)
                {
                    unitOfWork.Sounds.Remove(soundId);

                    var translationChangeSet = ChangeSetDto.Create(Operation.Delete, userId, RecordType.Sound, soundId);
                    unitOfWork.ChangeSets.Add(translationChangeSet);
                }

                // Process removed entry
                unitOfWork.Entries.Remove(entryId);

                var changeSet = ChangeSetDto.Create(Operation.Delete, userId, RecordType.Entry, entryId);
                unitOfWork.ChangeSets.Add(changeSet);

                unitOfWork.Commit();

                return new RequestResult() { Success = true };
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

            return new RequestResult() { Success = false };
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