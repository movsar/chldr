using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_utils;
using chldr_utils.Models;
using chldr_data.Services;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.Services;
using chldr_data.Repositories;
using chldr_data.local.Repositories;
using chldr_data.Responses;
using Realms.Sync;

namespace chldr_shared.Stores
{
    public class ContentStore
    {
        #region Events
        public event Action? ContentInitialized;
        public event Action? CachedResultsChanged;

        // ! Are these needed?
        public event Action<EntryModel>? EntryUpdated;
        public event Action<EntryModel>? EntryInserted;
        public event Action<EntryModel>? EntryDeleted;
        public event Action<EntryModel>? EntryAdded;
        #endregion

        #region Fields and Properties
        private readonly ExceptionHandler _exceptionHandler;
        private readonly IDataProvider _dataProvider;
        private readonly ISearchService _searchService;
        private readonly RequestService _requestService;
        // This shouldn't be normally used, but only to request models that have already been loaded 
        public SearchResultModel CachedSearchResult { get; set; } = new SearchResultModel(new List<EntryModel>());
        public readonly List<LanguageModel> Languages = LanguageModel.GetAvailableLanguages();
        #endregion

        #region EventHandlers
        private void DataAccess_GotNewSearchResults(SearchResultModel searchResult)
        {
            if (CachedSearchResult.SearchQuery == searchResult?.SearchQuery)
            {
                CachedSearchResult.Entries.Clear();

                foreach (var entry in searchResult.Entries)
                {
                    CachedSearchResult.Entries.Add(entry);
                }

                CachedResultsChanged?.Invoke();
                return;
            }

            CachedSearchResult = searchResult;
            CachedResultsChanged?.Invoke();
        }
        #endregion

        public ContentStore(ExceptionHandler exceptionHandler,
                            IDataProvider dataProvider,
                            ISearchService searchService,
                            RequestService requestService
            )
        {
            _exceptionHandler = exceptionHandler;
            _dataProvider = dataProvider;
            _searchService = searchService;
            _requestService = requestService;
            _searchService.GotNewSearchResult += DataAccess_GotNewSearchResults;

            _dataProvider.DatabaseInitialized += DataAccess_DatasourceInitialized;
            _dataProvider.Initialize();
        }

        public IEnumerable<EntryModel> Find(string inputText, int limit = 10)
        {
            return _searchService.Find(inputText, limit);
        }

        public void StartSearch(string inputText, FiltrationFlags filterationFlags)
        {
            Task.Run(() => _searchService.FindAsync(inputText, filterationFlags));
        }

        public async Task LoadRandomEntries()
        {
            CachedSearchResult.Entries.Clear();
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var entries = await unitOfWork.Entries.GetRandomsAsync(50);

            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }
        public void LoadLatestEntries()
        {
            CachedSearchResult.Entries.Clear();
            var entries = _searchService.GetLatestEntries();

            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }
        public void LoadEntriesToFiddleWith()
        {
            CachedSearchResult.Entries.Clear();
            var entries = _searchService.GetWordsToFiddleWith();

            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }
        public void LoadEntriesOnModeration()
        {
            CachedSearchResult.Entries.Clear();
            var entries = _searchService.GetEntriesOnModeration();

            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }
        public async Task<EntryModel> GetByEntryId(string entryId)
        {
            var unitOfWork = (RealmUnitOfWork)_dataProvider.CreateUnitOfWork();
            return await unitOfWork.Entries.Get(entryId);
        }
        public void DataAccess_DatasourceInitialized()
        {
            ContentInitialized?.Invoke();
        }

        public void Search(string query)
        {
            StartSearch(query, new FiltrationFlags());
        }
        public EntryModel GetCachedEntryById(string phraseId)
        {
            // Get current Phrase from cached results
            var phrase = CachedSearchResult.Entries
                .Where(e => e.Type == EntryType.Phrase)
                .Cast<EntryModel>()
                .FirstOrDefault(w => w.EntryId == phraseId);

            if (phrase == null)
            {
                throw _exceptionHandler.Error("Error:Phrase_shouldn't_be_null");
            }

            return phrase;
        }

        public async Task DeleteEntry(UserModel loggedInUser, string entryId)
        {
            // TODO: Change to call first the chosen repositories method, and only if Environment is !Web - change the realm db

            // Remove remote entity
            var response = await _requestService.RemoveEntry(loggedInUser.UserId, entryId);
            if (!response.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }
            var changeSets = RequestResult.GetData<IEnumerable<ChangeSetDto>>(response);

            // Remove local entity
            var unitOfWork = _dataProvider.CreateUnitOfWork(loggedInUser.UserId);

            RealmEntriesRepository entriesRepository = ((RealmEntriesRepository)unitOfWork.Entries);
            RealmChangeSetsRepository changeSetsRepository = ((RealmChangeSetsRepository)unitOfWork.ChangeSets);
            await entriesRepository.Remove(entryId, loggedInUser.UserId);
            await changeSetsRepository.AddRange(changeSets, loggedInUser.UserId);

            // Update on UI
            CachedSearchResult.Entries.Remove(CachedSearchResult.Entries.First(e => e.EntryId == entryId));
            CachedResultsChanged?.Invoke();
        }

        public async Task UpdateEntry(UserModel loggedInUser, EntryDto entryDto)
        {
            var userId = loggedInUser.UserId;

            // TODO: Change to call first the chosen repositories method, and only if Environment is !Web - change the realm db

            if (string.IsNullOrWhiteSpace(entryDto.ParentEntryId) && entryDto.Translations.Count() > 0)
            {
                throw new Exception("Error:Translations_not_allowed_for_subentries");
            }
            var response = await _requestService.UpdateEntry(loggedInUser.UserId, entryDto);
            if (!response.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }

            var changeSets = RequestResult.GetData<IEnumerable<ChangeSetDto>>(response);

            // Update local entity
            var unitOfWork = _dataProvider.CreateUnitOfWork(loggedInUser.UserId);
            RealmEntriesRepository entriesRepository = ((RealmEntriesRepository)unitOfWork.Entries);
            RealmChangeSetsRepository changeSetsRepository = ((RealmChangeSetsRepository)unitOfWork.ChangeSets);

            await entriesRepository.Update(entryDto, userId);
            await changeSetsRepository.AddRange(changeSets, userId);

            // Update on UI
            var existingEntry = CachedSearchResult.Entries.First(e => e.EntryId == entryDto.EntryId);

            var entryIndex = CachedSearchResult.Entries.IndexOf(existingEntry);
            CachedSearchResult.Entries[entryIndex] =await unitOfWork.Entries.Get(entryDto.EntryId);

            CachedResultsChanged?.Invoke();
        }
        public static async Task HandleUpdatedEntryTranslations(RealmTranslationsRepository translations, EntryDto existingEntryDto, EntryDto updatedEntryDto, string userId)
        {
            // Handle associated translation changes
            var existingEntryTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedEntryTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var added = updatedEntryDto.Translations.Where(t => !existingEntryTranslationIds.Contains(t.TranslationId));
            var deleted = existingEntryDto.Translations.Where(t => !updatedEntryTranslationIds.Contains(t.TranslationId));
            var updated = updatedEntryDto.Translations.Where(t => existingEntryTranslationIds.Contains(t.TranslationId) && updatedEntryTranslationIds.Contains(t.TranslationId));

            await translations.AddRange(added, userId);
            await translations.RemoveRange(deleted.Select(t => t.TranslationId), userId);
            await translations.UpdateRange(updated, userId);
        }

        public static async Task HandleUpdatedEntrySounds(RealmSoundsRepository sounds, EntryDto existingEntryDto, EntryDto updatedEntryDto, string userId)
        {
            // Handle associated translation changes
            var existingEntrySoundIds = existingEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();
            var updatedEntrySoundIds = updatedEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();

            var added = updatedEntryDto.Sounds.Where(t => !existingEntrySoundIds.Contains(t.SoundId));
            var deleted = existingEntryDto.Sounds.Where(t => !updatedEntrySoundIds.Contains(t.SoundId));
            var updated = updatedEntryDto.Sounds.Where(t => existingEntrySoundIds.Contains(t.SoundId) && updatedEntrySoundIds.Contains(t.SoundId));

            await sounds.AddRange(added, userId);
            await sounds.UpdateRange(updated, userId);
            await sounds.RemoveRange(deleted.Select(t => t.SoundId), userId);
        }
        public async Task AddEntry(UserModel loggedInUser, EntryDto entryDto)
        {
            if (string.IsNullOrWhiteSpace(entryDto.ParentEntryId) && entryDto.Translations.Count() > 0)
            {
                throw new Exception("Error:Translations_not_allowed_for_subentries");
            }

            // TODO: Set Rate of the entry and translation(s)

            var unitOfWork = _dataProvider.CreateUnitOfWork(loggedInUser.UserId);
            var resultingChangeSets = await unitOfWork.Entries.Add(entryDto, loggedInUser.UserId);

            var added = await unitOfWork.Entries.Get(entryDto.EntryId);
            CachedSearchResult.Entries.Add(added);
            CachedResultsChanged?.Invoke();
        }
    }
}