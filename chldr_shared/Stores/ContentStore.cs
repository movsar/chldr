using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_utils;
using chldr_utils.Models;
using chldr_utils.Services;

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
        #endregion

        #region Fields and Properties
        private readonly IUnitOfWork _unitOfWork;

        // This shouldn't be normally used, but only to request models that have already been loaded 
        public SearchResultModel CachedSearchResult { get; set; } = new SearchResultModel(new List<EntryModel>());
        public List<LanguageModel> Languages { get; } = new();
        // These are the sources excluding userIds
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
                            ISearchService searchService)
        {
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _dataProvider = dataProvider;

            searchService.GotNewSearchResult += DataAccess_GotNewSearchResults;
            //EntryUpdated += EntriesRepository_EntryUpdated;
            //_wordWriter.WordUpdated += EntriesRepository_WordUpdated;

            _dataProvider.LocalDatabaseInitialized += DataAccess_DatasourceInitialized;
            _dataProvider.Initialize();
        }

        private async void EntriesRepository_WordUpdated(WordModel updatedWord)
        {
            if (!CachedSearchResult.Entries.Any(e => e.EntryId == updatedWord.EntryId))
            {
                return;
            }

            var updatedWordIndex = CachedSearchResult.Entries.IndexOf(CachedSearchResult.Entries.First(e => e.EntryId == updatedWord.EntryId));

            CachedSearchResult.Entries[updatedWordIndex] = updatedWord;
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
            //Task.Run(() => _searchService.FindAsync(inputText, filterationFlags));
        }
        public void DeleteEntry(string entryId)
        {
            //EntriesRepository.Delete(entryId);
            CachedSearchResult.Entries.Remove(CachedSearchResult.Entries.First(e => e.EntryId == entryId));
            CachedResultsChanged?.Invoke();
        }
        public void LoadRandomEntries()
        {
            CachedSearchResult.Entries.Clear();
            //var entries = _searchService.GetRandomEntries();

            //CachedSearchResult.Entries.Clear();
            //foreach (var entry in entries)
            //{
            //    CachedSearchResult.Entries.Add(entry);
            //}

            //CachedResultsChanged?.Invoke();
        }
        public void LoadEntriesToFiddleWith()
        {
            //CachedSearchResult.Entries.Clear();
            //var entries = _searchService.GetWordsToFiddleWith();

            //CachedSearchResult.Entries.Clear();
            //foreach (var entry in entries)
            //{
            //    CachedSearchResult.Entries.Add(entry);
            //}

            //CachedResultsChanged?.Invoke();
        }
        public void LoadEntriesOnModeration()
        {
            //CachedSearchResult.Entries.Clear();
            //var entries = _searchService.GetEntriesOnModeration();

            //CachedSearchResult.Entries.Clear();
            //foreach (var entry in entries)
            //{
            //    CachedSearchResult.Entries.Add(entry);
            //}

            //CachedResultsChanged?.Invoke();
        }
        //public WordModel GetWordById(string entryId)
        //{
        //    return _wordQueries.GetById(entryId);
        //}
        //public PhraseModel GetPhraseById(string entryId)
        //{
        //    return _phraseQueries.GetById(entryId);
        //}

        //public EntryModel GetEntryById(string entryId)
        //{
        //    var word = _wordQueries.GetByEntryId(entryId);
        //    if (word != null)
        //    {
        //        return word;
        //    }

        //    var phrase = _phraseQueries.GetByEntryId(entryId);
        //    if (phrase != null)
        //    {
        //        return phrase;
        //    }

        //    return null;
        //}
        #endregion

        public void DataAccess_DatasourceInitialized()
        {
            if (Languages.Count == 0)
            {
                //Languages.AddRange(_unitOfWork.GetAllLanguages());
            }

            ContentInitialized?.Invoke();
        }

        public void Search(string query)
        {
            Search(query, new FiltrationFlags());
        }

        public PhraseModel AddNewPhrase(IUser userModel, string content, string notes)
        {
            //PhraseModel phrase = _phraseChangeRequests.Add(content, notes);
            return null;
        }

        public PhraseModel GetCachedPhraseById(string phraseId)
        {
            // Get current Phrase from cached results
            var phrase = CachedSearchResult.Entries
                .Where(e => (EntryType)e.Type == EntryType.Phrase)
                .Cast<PhraseModel>()
                .FirstOrDefault(w => w.PhraseId == phraseId);

            if (phrase == null)
            {
                var npe = new Exception("Error:Phrase_shouldn't_be_null");
                _exceptionHandler.LogAndThrow(npe);
                throw npe;
            }

            return phrase;
        }

        public void UpdatePhrase(UserModel loggedInUser, string? phraseId, string? content, string? notes)
        {
            // _dataAccess.UpdatePhrase(loggedInUser, phraseId, content, notes);
        }

        public async Task UpdateWord(UserModel loggedInUser, WordDto word)
        {
            //await _wordWriter.Update(loggedInUser, word);
        }
    }
}