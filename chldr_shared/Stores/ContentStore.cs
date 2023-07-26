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

        // ! This shouldn't be normally used, but only to request models that have already been loaded 
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

        public ContentStore(ExceptionHandler exceptionHandler, IDataProvider dataProvider)
        {
            _exceptionHandler = exceptionHandler;
            _dataProvider = dataProvider;

            var unitOfWork = _dataProvider.CreateUnitOfWork();
            unitOfWork.Entries.GotNewSearchResult += DataAccess_GotNewSearchResults;

            _dataProvider.DatabaseInitialized += DataAccess_DatasourceInitialized;
            _dataProvider.Initialize();
        }

        public IEnumerable<EntryModel> Find(string inputText, int limit = 10)
        {
            //return await _searchService.FindAsync(inputText, limit);
            throw new NotImplementedException();
        }

        public void StartSearch(string inputText, FiltrationFlags filterationFlags)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            unitOfWork.Entries.FindDeferredAsync(inputText.Replace("1", "Ӏ").ToLower(), filterationFlags);
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
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var entries = unitOfWork.Entries.GetLatestEntries();

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
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var entries = unitOfWork.Entries.GetWordsToFiddleWith();

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
            var unitOfWork = _dataProvider.CreateUnitOfWork();
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
            var unitOfWork = _dataProvider.CreateUnitOfWork();
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
            var unitOfWork = _dataProvider.CreateUnitOfWork(loggedInUser.UserId);
            await unitOfWork.Entries.Remove(entryId, loggedInUser.UserId);

            // Update on UI
            CachedSearchResult.Entries.Remove(CachedSearchResult.Entries.First(e => e.EntryId == entryId));
            CachedResultsChanged?.Invoke();
        }

        public async Task UpdateEntry(UserModel loggedInUser, EntryDto entryDto)
        {
            if (string.IsNullOrWhiteSpace(entryDto.ParentEntryId) && entryDto.Translations.Count() > 0)
            {
                throw new Exception("Error:Translations_not_allowed_for_subentries");
            }

            var unitOfWork = _dataProvider.CreateUnitOfWork(loggedInUser.UserId);
            await unitOfWork.Entries.Update(entryDto, loggedInUser.UserId);

            // Update UI
            var existingEntry = CachedSearchResult.Entries.First(e => e.EntryId == entryDto.EntryId);
            var entryIndex = CachedSearchResult.Entries.IndexOf(existingEntry);
            CachedSearchResult.Entries[entryIndex] =await unitOfWork.Entries.Get(entryDto.EntryId);

            CachedResultsChanged?.Invoke();
        }
        public async Task AddEntry(UserModel loggedInUser, EntryDto entryDto)
        {
            if (string.IsNullOrWhiteSpace(entryDto.ParentEntryId) && entryDto.Translations.Count() > 0)
            {
                throw new Exception("Error:Translations_not_allowed_for_subentries");
            }

            var unitOfWork = _dataProvider.CreateUnitOfWork(loggedInUser.UserId);
            await unitOfWork.Entries.Add(entryDto, loggedInUser.UserId);
         
            // Update UI
            var added = await unitOfWork.Entries.Get(entryDto.EntryId);
            CachedSearchResult.Entries.Add(added);
            CachedResultsChanged?.Invoke();
        }
    }
}