using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_utils;
using chldr_utils.Services;
using dosham.Services;
using chldr_data.DatabaseObjects.Interfaces;

namespace dosham.Stores
{
    public class ContentStore
    {

        #region Events
        public event Action? ContentInitialized;
        public event Action? CachedResultsChanged;
        #endregion

        #region Fields and Properties
        private readonly ExceptionHandler _exceptionHandler;
        private readonly IDataProvider _dataProvider;
        private readonly IEnvironmentService _environmentService;

        // ! This shouldn't be normally used, but only to request models that have already been loaded 
        public SearchResultModel CachedSearchResult { get; set; } = new SearchResultModel(new List<EntryModel>());
        public readonly List<LanguageModel> Languages = LanguageModel.GetAvailableLanguages();

        public EntryService EntryService;
        public UserService UserService;
        public SourceService SourceService;
        #endregion

        #region EventHandlers
        private void EntryService_OnNewSearchResults(SearchResultModel searchResult)
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

        public ContentStore(
            ExceptionHandler exceptionHandler,
            IDataProvider dataProvider,
            IEnvironmentService environmentService,

            SourceService sourceService,
            EntryService entryService,
            UserService userService
            )
        {
            _exceptionHandler = exceptionHandler;
            _dataProvider = dataProvider;
            _environmentService = environmentService;

            var unitOfWork = _dataProvider.Repositories(null);

            _dataProvider.DatabaseInitialized += DataAccess_DatasourceInitialized;
            _dataProvider.Initialize();

            EntryService = entryService;
            SourceService = sourceService;
            UserService = userService;

            EntryService.EntryUpdated += OnEntryUpdated;
            EntryService.EntryInserted += OnEntryInserted;
            EntryService.EntryRemoved += OnEntryRemoved;
            EntryService.NewDeferredSearchResult += EntryService_OnNewSearchResults;
        }

        private async Task OnEntryUpdated(EntryModel entry)
        {
            var existingEntry = CachedSearchResult.Entries.First(e => e.EntryId == entry.EntryId || e.EntryId == entry.ParentEntryId);
            var entryIndex = CachedSearchResult.Entries.IndexOf(existingEntry);
            CachedSearchResult.Entries[entryIndex] = await EntryService.GetAsync(entry.EntryId);
            CachedResultsChanged?.Invoke();
        }
        private async Task OnEntryRemoved(EntryModel entry)
        {
            // Update on UI
            CachedSearchResult.Entries.Remove(CachedSearchResult.Entries.First(e => e.EntryId == entry.EntryId));
            CachedResultsChanged?.Invoke();
        }
        private async Task OnEntryInserted(EntryModel entry)
        {
            LoadLatestEntries();
        }

        public async Task LoadRandomEntries()
        {
            var entries = await EntryService.GetRandomsAsync(50);
            
            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }
        public async void LoadLatestEntries()
        {
            var unitOfWork = _dataProvider.Repositories();
            var entries = await unitOfWork.Entries.GetLatestEntriesAsync(50);

            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }

        public async void LoadEntriesOnModeration()
        {
            CachedSearchResult.Entries.Clear();
            var unitOfWork = _dataProvider.Repositories(null);
            var entries = await unitOfWork.Entries.GetEntriesOnModerationAsync();

            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }
      
        public void DataAccess_DatasourceInitialized()
        {
            ContentInitialized?.Invoke();
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
    }
}