using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.realm.Repositories;
using chldr_data.realm.Services;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Responses;
using chldr_data.Services;
using chldr_utils;
using IEntry = chldr_data.DatabaseObjects.Interfaces.IEntry;
namespace dosham.Services
{
    public class EntryService : IDboService<EntryModel, EntryDto>
    {
        internal event Action<EntryModel> EntryUpdated;
        internal event Action<EntryModel> EntryInserted;
        internal event Action<EntryModel> EntryRemoved;

        private readonly IDataProvider _dataProvider;
        private readonly RequestService _requestService;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly IEnvironmentService _environmentService;

        public EntryService(
            IDataProvider dataProvider,
            RequestService requestService,
            ExceptionHandler exceptionHandler,
            IEnvironmentService environmentService)
        {
            _dataProvider = dataProvider;
            _requestService = requestService;
            _exceptionHandler = exceptionHandler;
            _environmentService = environmentService;
        }

        #region Get
        public async Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags)
        {
            var repositories = _dataProvider.Repositories();
            var entries = await repositories.Entries.TakeAsync(offset, limit, filtrationFlags);
            return entries.ToList();
        }

        public async Task<int> GetCountAsync(FiltrationFlags filtrationFlags)
        {
            var repositories = _dataProvider.Repositories();
            return await repositories.Entries.CountAsync(filtrationFlags);
        }
        public async Task<List<EntryModel>> FindAsync(string inputText)
        {
            var repositories = _dataProvider.Repositories();
            return (await repositories.Entries.FindAsync(inputText)).ToList();
        }

        public async Task<EntryModel> GetAsync(string entryId)
        {
            var repositories = _dataProvider.Repositories();
            return await repositories.Entries.GetAsync(entryId);
        }
        #endregion

        #region Add
        private async Task AddToLocalDatabase(EntryDto newEntryDto, string userId, IEnumerable<ChangeSetDto> changeSets)
        {
            var repositories = (RealmDataAccessor)_dataProvider.Repositories(userId);
            var entriesRepository = (RealmEntriesRepository)repositories.Entries;
            var changeSetsRepository = (RealmChangeSetsRepository)repositories.ChangeSets;

            await entriesRepository.Add(newEntryDto);
            await changeSetsRepository.AddRange(changeSets);
        }

        private async Task<InsertResponse> AddToRemmoteDatabase(EntryDto newEntryDto, string userId)
        {
            if (newEntryDto == null || string.IsNullOrEmpty(newEntryDto.EntryId))
            {
                throw new NullReferenceException();
            }

            // Insert remote entity with translations
            var response = await _requestService.AddEntryAsync(newEntryDto);
            if (!response.Success)
            {
                throw _exceptionHandler.Error(response.ErrorMessage);
            }
            var responseData = RequestResult.GetData<InsertResponse>(response);
            if (responseData.CreatedAt == DateTimeOffset.MinValue)
            {
                throw _exceptionHandler.Error(response.ErrorMessage);
            }
            return responseData;
        }

        public async Task AddAsync(EntryDto entryDto, string userId)
        {
            try
            {
                var insertResponse = await AddToRemmoteDatabase(entryDto, userId);

                if (_dataProvider is RealmDataProvider)
                {
                    await AddToLocalDatabase(entryDto, userId, insertResponse.ChangeSets.Select(ChangeSetDto.FromModel));
                }

                var repositories = _dataProvider.Repositories();
                var entry = await repositories.Entries.GetAsync(entryDto.EntryId);
                EntryInserted?.Invoke(entry);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Update
        public async Task PromoteTranslationAsync(ITranslation translationInfo, UserModel? currentUser)
        {
            // TODO: Use requestService
            var repositories = _dataProvider.Repositories(currentUser.Id);
            await repositories.Translations.Promote(translationInfo);
        }

        public async Task PromotePronunciationAsync(IPronunciation soundInfo, UserModel? currentUser)
        {
            var repositories = _dataProvider.Repositories(currentUser.Id);
            await repositories.Sounds.Promote(soundInfo);
        }
        private async Task<UpdateResponse> PromoteRequestAsync(IEntry entry, UserModel? currentUser)
        {
            var response = await _requestService.PromoteAsync(RecordType.Entry, entry.EntryId);
            if (!response.Success)
            {
                throw _exceptionHandler.Error(response.ErrorMessage);
            }
            var responseData = RequestResult.GetData<UpdateResponse>(response);

            return responseData;
        }
        public async Task PromoteAsync(IEntry entry, UserModel? currentUser)
        {
            await PromoteRequestAsync(entry, currentUser);

            // TODO: Upate local, if used
            EntryUpdated?.Invoke((EntryModel)entry);
        }

        private async Task UpdateInLocalDatabase(EntryDto newEntryDto, string userId, IEnumerable<ChangeSetDto> changeSets)
        {
            var repositories = (RealmDataAccessor)_dataProvider.Repositories(userId);
            var entriesRepository = (RealmEntriesRepository)repositories.Entries;
            var changeSetsRepository = (RealmChangeSetsRepository)repositories.ChangeSets;

            await entriesRepository.Update(newEntryDto);
            await changeSetsRepository.AddRange(changeSets);
        }

        private async Task<UpdateResponse> UpdateInRemmoteDatabase(EntryDto updatedEntryDto, string userId)
        {
            var response = await _requestService.UpdateEntry(updatedEntryDto);
            if (!response.Success)
            {
                throw _exceptionHandler.Error(response.ErrorMessage);
            }
            var responseData = RequestResult.GetData<UpdateResponse>(response);

            return responseData;
        }

        public async Task UpdateAsync(EntryDto entryDto, string userId)
        {
            try
            {
                var updateResponse = await UpdateInRemmoteDatabase(entryDto, userId);

                if (_dataProvider is RealmDataProvider)
                {
                    await UpdateInLocalDatabase(entryDto, userId, updateResponse.ChangeSets.Select(ChangeSetDto.FromModel));
                }

                var repositories = _dataProvider.Repositories();
                var updatedEntry = await repositories.Entries.GetAsync(entryDto.EntryId);
                EntryUpdated?.Invoke(updatedEntry);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Remove
        private async Task<UpdateResponse> RemoveFromRemmoteDatabase(EntryModel entry, string userId)
        {
            var response = await _requestService.RemoveEntry(entry.EntryId);
            if (!response.Success)
            {
                throw _exceptionHandler.Error(response.ErrorMessage);
            }
            var responseData = RequestResult.GetData<UpdateResponse>(response);
            return responseData;
        }
        private async Task RemoveFromLocalDatabase(EntryModel entry, string userId, IEnumerable<ChangeSetDto> changeSets)
        {
            var repositories = (RealmDataAccessor)_dataProvider.Repositories(userId);
            var entriesRepository = (RealmEntriesRepository)repositories.Entries;
            var soundsRepository = (RealmSoundsRepository)repositories.Sounds;
            var translationsRepository = (RealmTranslationsRepository)repositories.Translations;
            var changeSetsRepository = (RealmChangeSetsRepository)repositories.ChangeSets;

            var sounds = entry.Sounds.Select(s => s.SoundId).ToArray();
            var translations = entry.Translations.Select(t => t.TranslationId).ToArray();

            await soundsRepository.RemoveRange(sounds);
            await translationsRepository.RemoveRange(translations);
            await entriesRepository.Remove(entry.EntryId);

            await changeSetsRepository.AddRange(changeSets);
        }
        public async Task RemoveAsync(EntryModel entry, string userId)
        {
            try
            {
                var updateResponse = await RemoveFromRemmoteDatabase(entry, userId);

                if (_dataProvider is RealmDataProvider)
                {
                    await RemoveFromLocalDatabase(entry, userId, updateResponse.ChangeSets.Select(ChangeSetDto.FromModel));
                }

                EntryRemoved?.Invoke(entry);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        public async Task<List<EntryModel>> GetRandomsAsync(int count)
        {
            var entries = new List<EntryModel>();
            if (_environmentService.CurrentPlatform == Platforms.Web)
            {
                var response = await _requestService.GetRandomsAsync(count);
                if (!response.Success)
                {
                    throw _exceptionHandler.Error(response.ErrorMessage);
                }
                entries = RequestResult.GetData<List<EntryModel>>(response);
            }
            else
            {
                var repositories = _dataProvider.Repositories(null);
                entries = await repositories.Entries.GetRandomsAsync(50);
            }

            return entries;
        }
    }
}
