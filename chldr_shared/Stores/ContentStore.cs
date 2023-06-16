using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_utils;
using chldr_utils.Models;
using chldr_shared.Services;

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
        private IUnitOfWork _unitOfWork;
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

        public void LoadRandomEntries()
        {
            CachedSearchResult.Entries.Clear();
            var entries = _searchService.GetRandomEntries();

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
        public EntryModel GetByEntryId(string entryId)
        {
            return _unitOfWork.Entries.Get(entryId);
        }
        public void DataAccess_DatasourceInitialized()
        {
            _unitOfWork = _dataProvider.CreateUnitOfWork();

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
            // Remove remote entity
            var request = await _requestService.RemoveEntry(loggedInUser.UserId, entryId);
            if (!request.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }

            // Remove local entity
            _unitOfWork.Entries.Remove(entryId);

            // Update on UI
            CachedSearchResult.Entries.Remove(CachedSearchResult.Entries.First(e => e.EntryId == entryId));
            CachedResultsChanged?.Invoke();
        }

        public async Task UpdateEntry(UserModel loggedInUser, EntryDto entryDto)
        {
            var request = await _requestService.UpdateEntry(loggedInUser.UserId, entryDto);
            if (!request.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }

            // Update local entity
            _unitOfWork.Entries.Update(entryDto);

            // Update on UI
            var existingEntry = CachedSearchResult.Entries.First(e => e.EntryId == entryDto.EntryId);

            var entryIndex = CachedSearchResult.Entries.IndexOf(existingEntry);
            CachedSearchResult.Entries[entryIndex] = _unitOfWork.Entries.Get(entryDto.EntryId);

            CachedResultsChanged?.Invoke();
        }

        public async Task AddEntry(UserModel loggedInUser, EntryDto entryDto)
        {
            // Add remote entity
            var request = await _requestService.AddEntry(loggedInUser.UserId, entryDto);
            if (!request.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }

            // Update local entity
            entryDto.CreatedAt = request.CreatedAt;
            _unitOfWork.Entries.Add(entryDto);

            CachedSearchResult.Entries.Add(_unitOfWork.Entries.Get(entryDto.EntryId));
            CachedResultsChanged?.Invoke();
        }
    }
}