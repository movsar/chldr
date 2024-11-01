﻿using domain.Interfaces;
using domain.Models;
using domain.DatabaseObjects.Models;
using chldr_app.Services;
using MailKit.Search;

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

        private List<EntryModel> _searchResults = new List<EntryModel>();
        public List<EntryModel> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                SearchResultsReady?.Invoke(_searchResults);
            }
        }

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

        public void FindEntryDeferred(string inputText, FiltrationFlags? filtrationFlags)
        {
            // Run asynchronously
            _ = Task.Run(async () =>
            {
                // Check if the results are already in cache, if not, retrieve
                var entries = _entryCache.Get(inputText);
                if (entries == null)
                {
                    entries = await EntryService.FindAsync(inputText, filtrationFlags);
                    _entryCache.Add(inputText, entries);
                }

                SearchResults = entries;
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
                SearchResults = await EntryService.GetRandomsEntriesAsync(50);
            });
        }
        public void RequestLatestEntries()
        {
            _ = Task.Run(async () =>
            {
                SearchResults = await _dataProvider.Repositories(null).Entries.GetLatestEntriesAsync(50);
            });
        }

        public void RequestEntriesOnModeration()
        {
            _ = Task.Run(async () =>
            {
                SearchResults = await _dataProvider.Repositories(null).Entries.GetEntriesOnModerationAsync();
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