using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Interfaces;
using core.DatabaseObjects.Models;
using core.Enums;
using core.Interfaces;
using core.Models;
using core.Responses;
using core.Services;
using IEntry = core.DatabaseObjects.Interfaces.IEntry;
namespace chldr_app.Services
{
    public class EntryService : IDboService<EntryModel, EntryDto>
    {
        internal event Action<EntryModel> EntryUpdated;
        internal event Action<EntryModel> EntryInserted;
        internal event Action<EntryModel> EntryRemoved;

        private readonly IDataProvider _dataProvider;
        private readonly IRequestService _requestService;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IEnvironmentService _environmentService;

        public EntryService(
            IDataProvider dataProvider,
            IRequestService requestService,
            IExceptionHandler exceptionHandler,
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
        public async Task<List<EntryModel>> FindAsync(string inputText, FiltrationFlags? filtrationFlags = null)
        {
            var repositories = _dataProvider.Repositories();
            return (await repositories.Entries.FindAsync(inputText, filtrationFlags)).ToList();
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
            var repositories = _dataProvider.Repositories(userId);
            var entriesRepository = repositories.Entries;
            var changeSetsRepository = repositories.ChangeSets;

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

                await AddToLocalDatabase(entryDto, userId, insertResponse.ChangeSets.Select(ChangeSetDto.FromModel));

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
            var repositories = _dataProvider.Repositories(currentUser!.Id);
            await repositories.Translations.Promote(translationInfo);
        }

        public async Task PromotePronunciationAsync(ISound soundInfo, UserModel? currentUser)
        {
            var repositories = _dataProvider.Repositories(currentUser!.Id);
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
            var repositories = _dataProvider.Repositories(userId);
            var entriesRepository = repositories.Entries;
            var changeSetsRepository = repositories.ChangeSets;

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

                await UpdateInLocalDatabase(entryDto, userId, updateResponse.ChangeSets.Select(ChangeSetDto.FromModel));

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
            var repositories = _dataProvider.Repositories(userId);
            var entriesRepository = repositories.Entries;
            var soundsRepository = repositories.Sounds;
            var translationsRepository = repositories.Translations;
            var changeSetsRepository = repositories.ChangeSets;

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

                await RemoveFromLocalDatabase(entry, userId, updateResponse.ChangeSets.Select(ChangeSetDto.FromModel));

                EntryRemoved?.Invoke(entry);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        public async Task<List<EntryModel>> GetRandomsEntriesAsync(int count)
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
