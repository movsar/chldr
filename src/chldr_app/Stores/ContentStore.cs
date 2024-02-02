using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Models;
using chldr_app.Services;

namespace chldr_app.Stores
{
    public class ContentStore
    {
        #region Events, Fields, Properties and Constructors
        public event Action? ContentInitialized;
        public event Action<List<EntryModel>>? SearchResultsReady;

        private readonly EntryCacheService _entryCache;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IDataProvider _dataProvider;
        private readonly IEnvironmentService _environmentService;
        public readonly List<LanguageModel> Languages = LanguageModel.GetAvailableLanguages();

        public EntryService EntryService;
        public UserService UserService;
        public SourceService SourceService;

        public ContentStore(
            IExceptionHandler exceptionHandler,
            IDataProvider dataProvider,
            IEnvironmentService environmentService,

            SourceService sourceService,
            EntryService entryService,
            UserService userService,
            EntryCacheService entryCacheService
            )
        {
            _exceptionHandler = exceptionHandler;
            _dataProvider = dataProvider;
            _environmentService = environmentService;
            _entryCache = entryCacheService;

            _dataProvider.DatabaseInitialized += DataAccess_DatasourceInitialized;

            EntryService = entryService;
            SourceService = sourceService;
            UserService = userService;

            EntryService.EntryUpdated += OnEntryUpdated;
            EntryService.EntryInserted += OnEntryInserted;
            EntryService.EntryRemoved += OnEntryRemoved;
        }
        #endregion

        public void FindEntryDeferred(string inputText)
        {
            // Run asynchronously
            _ = Task.Run(async () =>
            {
                // Check if the results are already in cache, if not, retrieve
                var result = _entryCache.Get(inputText);
                if (result == null)
                {
                    result = await EntryService.FindAsync(inputText);
                    _entryCache.Add(inputText, result);
                }

                SearchResultsReady?.Invoke(result);
            });
        }

        private void OnEntryUpdated(EntryModel entry)
        {
            _entryCache.Update(entry);
        }
        private void OnEntryRemoved(EntryModel entry)
        {
            _entryCache.Remove(entry);
        }
        private void OnEntryInserted(EntryModel entry)
        {
            RequestLatestEntries();
        }

        public void RequestRandomEntries()
        {
            _ = Task.Run(async () =>
            {
                var entries = await EntryService.GetRandomsAsync(50);
                SearchResultsReady?.Invoke(entries);
            });
        }
        public void RequestLatestEntries()
        {
            _ = Task.Run(async () =>
            {
                var entries = await _dataProvider.Repositories(null).Entries.GetLatestEntriesAsync(50);
                SearchResultsReady?.Invoke(entries);
            });
        }

        public void RequestEntriesOnModeration()
        {
            _ = Task.Run(async () =>
            {
                var entries = await _dataProvider.Repositories(null).Entries.GetEntriesOnModerationAsync();
                SearchResultsReady?.Invoke(entries);
            });
        }

        public void Initialize()
        {
            _dataProvider.Initialize();
        }

        private void DataAccess_DatasourceInitialized()
        {
            ContentInitialized?.Invoke();
        }

    }
}