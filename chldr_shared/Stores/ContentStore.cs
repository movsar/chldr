using chldr_data.Dto;
using chldr_data.Enums;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Models.Words;
using chldr_data.Repositories;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Models;
using chldr_utils.Services;
using MongoDB.Bson;
using Realms;

namespace chldr_shared.Stores
{
    public class ContentStore
    {

        #region Events
        public event Action? ContentInitialized;
        public event Action? CachedResultsChanged;

        private readonly ExceptionHandler _exceptionHandler;
        private readonly NetworkService _networkService;
        #endregion

        #region Fields and Properties
        private readonly IDataAccess _dataAccess;
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
        public ContentStore(IDataAccess dataAccess, ExceptionHandler exceptionHandler, NetworkService networkService)
        {
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;

            _dataAccess = dataAccess;
            _dataAccess.EntriesRepository.GotNewSearchResult += DataAccess_GotNewSearchResults;
            _dataAccess.EntriesRepository.EntryUpdated += EntriesRepository_EntryUpdated;
            _dataAccess.WordsRepository.WordUpdated += EntriesRepository_WordUpdated;
            _dataAccess.DatasourceInitialized += DataAccess_DatasourceInitialized;
        }

        private async void EntriesRepository_WordUpdated(WordModel updatedWord)
        {
            if (!CachedSearchResult.Entries.Any(e => e.Id == ((EntryModel)updatedWord).Id))
            {
                return;
            }

            var updatedWordIndex = CachedSearchResult.Entries.IndexOf(CachedSearchResult.Entries.First(e => e.Id == ((EntryModel)updatedWord).Id));

            CachedSearchResult.Entries[updatedWordIndex] = updatedWord;
        }

        private void EntriesRepository_EntryUpdated(EntryModel entry)
        {
            var entryInCachedResults = CachedSearchResult.Entries.First(e => e.Id == entry.Id);
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
            Task.Run(() => _dataAccess.EntriesRepository.FindAsync(inputText, filterationFlags));
        }
        public void DeleteEntry(ObjectId entryId)
        {
            _dataAccess.EntriesRepository.Delete(entryId);
            CachedSearchResult.Entries.Remove(CachedSearchResult.Entries.First(e => e.Id == entryId));
            CachedResultsChanged?.Invoke();
        }
        public void LoadRandomEntries()
        {
            CachedSearchResult.Entries.Clear();
            var entries = _dataAccess.EntriesRepository.GetRandomEntries();

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
            var entries = _dataAccess.EntriesRepository.GetWordsToFiddleWith();

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
            var entries = _dataAccess.EntriesRepository.GetEntriesOnModeration();

            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }
        public WordModel GetWordById(ObjectId entryId)
        {
            return _dataAccess.WordsRepository.GetById(entryId);
        }
        public PhraseModel GetPhraseById(ObjectId entryId)
        {
            return _dataAccess.PhrasesRepository.GetById(entryId);
        }

        public EntryModel GetEntryById(ObjectId entryId)
        {
            var word = _dataAccess.WordsRepository.GetByEntryId(entryId);
            if (word != null)
            {
                return word;
            }

            var phrase = _dataAccess.PhrasesRepository.GetByEntryId(entryId);
            if (phrase != null)
            {
                return phrase;
            }

            return null;
        }
        #endregion

        public void DataAccess_DatasourceInitialized(DataSourceType dataSourceType)
        {
            if (Languages.Count == 0)
            {
                Languages.AddRange(_dataAccess.LanguagesRepository.GetAllLanguages());
            }

            ContentInitialized?.Invoke();

            try
            {
                if (dataSourceType == DataSourceType.Offline && _networkService.IsNetworUp)
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(250);
                        _dataAccess.ActivateDatasource(DataSourceType.Synced);
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("998"))
                {
                    _exceptionHandler.ProcessDebug(new Exception("NETWORK_ERROR"));
                }
                else
                {
                    _exceptionHandler.ProcessDebug(ex);
                }
            }
        }

        public void Search(string query)
        {
            Search(query, new FiltrationFlags());
        }

        public PhraseModel AddNewPhrase(UserModel userModel, string content, string notes)
        {
            PhraseModel phrase = _dataAccess.PhrasesRepository.Add(content, notes);
            return phrase;
        }

        public PhraseModel GetCachedPhraseById(ObjectId phraseId)
        {
            // Get current Phrase from cached results
            var phrase = CachedSearchResult.Entries
                .Where(e => (EntryType)e.Type == EntryType.Phrase)
                .Cast<PhraseModel>()
                .FirstOrDefault(w => w.PhraseId == phraseId);

            if (phrase == null)
            {
                var npe = new Exception("Error:Phrase_shouldn't_be_null");
                _exceptionHandler.ProcessError(npe);
                throw npe;
            }

            return phrase;
        }

        public void UpdatePhrase(UserModel loggedInUser, string? phraseId, string? content, string? notes)
        {
            // _dataAccess.UpdatePhrase(loggedInUser, phraseId, content, notes);
        }

        public void UpdateWord(UserModel user, WordDto word)
        {
            _dataAccess.WordsRepository.Update(user, word);
        }
    }
}