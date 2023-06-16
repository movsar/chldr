﻿using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_utils;
using chldr_utils.Models;
using chldr_utils.Services;
using chldr_shared.Services;

namespace chldr_shared.Stores
{
    public class ContentStore
    {

        #region Events
        public event Action? ContentInitialized;
        public event Action? CachedResultsChanged;

        private readonly ExceptionHandler _exceptionHandler;
        private readonly NetworkService _networkService;
        private readonly IDataProvider _dataProvider;
        private readonly ISearchService _searchService;
        private readonly RequestService _requestService;
        #endregion

        #region Fields and Properties
        private IUnitOfWork _unitOfWork;

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

        #region Constructors
        public ContentStore(ExceptionHandler exceptionHandler,
                            NetworkService networkService,
                            IDataProvider dataProvider,
                            ISearchService searchService,
                            RequestService requestService

            )
        {
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _dataProvider = dataProvider;
            _searchService = searchService;
            _requestService = requestService;
            _searchService.GotNewSearchResult += DataAccess_GotNewSearchResults;

            _dataProvider.EntryUpdated += EntriesRepository_EntryUpdated;
            _dataProvider.DatabaseInitialized += DataAccess_DatasourceInitialized;
            _dataProvider.Initialize();
        }

        private void EntriesRepository_EntryUpdated(EntryModel entry)
        {
            var entryInCachedResults = CachedSearchResult.Entries.First(e => e.EntryId == entry.EntryId);
            if (entryInCachedResults == null)
            {
                return;
            }

            entryInCachedResults = entry;
        }
        #endregion

        #region Methods
        public void Search(string inputText, FiltrationFlags filterationFlags)
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

        #endregion
        public void DataAccess_DatasourceInitialized()
        {
            _unitOfWork = _dataProvider.CreateUnitOfWork();

            ContentInitialized?.Invoke();
        }

        public void Search(string query)
        {
            Search(query, new FiltrationFlags());
        }
        public EntryModel GetCachedEntryById(string phraseId)
        {
            // Get current Phrase from cached results
            var phrase = CachedSearchResult.Entries
                .Where(e => (EntryType)e.Type == EntryType.Phrase)
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

            //CachedSearchResult.Entries.Remove(CachedSearchResult.Entries.First(e => e.EntryId == entryId));
            //CachedResultsChanged?.Invoke();
        }

        public async Task UpdateEntry(UserModel loggedInUser, EntryDto EntryDto)
        {
            var request = await _requestService.UpdateEntry(loggedInUser.UserId, EntryDto);
            if (!request.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }

            // Update local entity
            _unitOfWork.Entries.Update(EntryDto);
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
        }
    }
}