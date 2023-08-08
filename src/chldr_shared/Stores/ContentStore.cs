﻿using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_utils;
using chldr_utils.Services;
using chldr_shared.Services;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_shared.Stores
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
        private readonly EnvironmentService _environmentService;

        // ! This shouldn't be normally used, but only to request models that have already been loaded 
        public SearchResultModel CachedSearchResult { get; set; } = new SearchResultModel(new List<EntryModel>());
        public readonly List<LanguageModel> Languages = LanguageModel.GetAvailableLanguages();

        public EntryService EntryService;
        public TranslationService TranslationService;
        public PronunciationService PronunciationService;
        public UserService UserService;
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

        public ContentStore(
            ExceptionHandler exceptionHandler,
            IDataProvider dataProvider,
            EnvironmentService environmentService,
            EntryService entryService,
            TranslationService translationService,
            PronunciationService pronunciationService,
            UserService userService
            )
        {
            _exceptionHandler = exceptionHandler;
            _dataProvider = dataProvider;
            _environmentService = environmentService;

            var unitOfWork = _dataProvider.CreateUnitOfWork(null);
            unitOfWork.Entries.GotNewSearchResult += DataAccess_GotNewSearchResults;

            _dataProvider.DatabaseInitialized += DataAccess_DatasourceInitialized;
            _dataProvider.Initialize();

            EntryService = entryService;
            TranslationService = translationService;
            PronunciationService = pronunciationService;
            UserService = userService;

            EntryService.EntryUpdated += OnEntryUpdated;
            EntryService.EntryInserted += OnEntryInserted;
            EntryService.EntryRemoved += OnEntryRemoved;
        }

        private void OnEntryUpdated(EntryModel entry) { }
        private void OnEntryRemoved(EntryModel entry) { }
        private void OnEntryInserted(EntryModel entry) { }

        public async Task<IEnumerable<EntryModel>> FindAsync(string inputText, int limit = 10)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            return (await unitOfWork.Entries.FindAsync(inputText, new FiltrationFlags())).ToList();
        }

        public async Task StartSearch(string inputText, FiltrationFlags filterationFlags)
        {
            var query = inputText.Replace("1", "Ӏ").ToLower();

            var unitOfWork = _dataProvider.CreateUnitOfWork(null);

            if (_environmentService.CurrentPlatform == Enums.Platforms.Web)
            {
                var results = await unitOfWork.Entries.FindAsync(query, filterationFlags);

                CachedSearchResult.Entries.Clear();

                foreach (var entry in results)
                {
                    CachedSearchResult.Entries.Add(entry);
                }

                CachedResultsChanged?.Invoke();
            }
            else
            {
                await unitOfWork.Entries.FindDeferredAsync(query, filterationFlags);
            }
        }

        public async Task LoadRandomEntries()
        {
            CachedSearchResult.Entries.Clear();
            var unitOfWork = _dataProvider.CreateUnitOfWork(null);
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
            var unitOfWork = _dataProvider.CreateUnitOfWork(null);
            var entries = unitOfWork.Entries.GetLatestEntries();

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
            var unitOfWork = _dataProvider.CreateUnitOfWork(null);
            var entries = unitOfWork.Entries.GetEntriesOnModeration();

            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }
        public async Task<EntryModel> GetByEntryId(string entryId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(null);
            return await unitOfWork.Entries.GetAsync(entryId);
        }
        public void DataAccess_DatasourceInitialized()
        {
            ContentInitialized?.Invoke();
        }

        public async Task Search(string query)
        {
            await StartSearch(query, new FiltrationFlags());
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

        public async Task DeleteEntry(UserModel loggedInUser, EntryModel entry)
        {
            await EntryService.RemoveAsync(entry, loggedInUser.UserId);

            // Update on UI
            CachedSearchResult.Entries.Remove(CachedSearchResult.Entries.First(e => e.EntryId == entry.EntryId));
            //CachedResultsChanged?.Invoke();
        }

        public async Task UpdateEntry(UserModel loggedInUser, EntryDto entryDto)
        {
            await EntryService.UpdateAsync(entryDto, loggedInUser.UserId);

            // Update UI
            var existingEntry = CachedSearchResult.Entries.First(e => e.EntryId == entryDto.EntryId || e.EntryId == entryDto.ParentEntryId);
            var entryIndex = CachedSearchResult.Entries.IndexOf(existingEntry);
            CachedSearchResult.Entries[entryIndex] = await EntryService.GetAsync(entryDto.EntryId);
            //CachedResultsChanged?.Invoke();
        }
        public async Task AddEntry(UserModel loggedInUser, EntryDto entryDto)
        {
            await EntryService.AddAsync(entryDto, loggedInUser.UserId);

            LoadLatestEntries();
        }
    }
}