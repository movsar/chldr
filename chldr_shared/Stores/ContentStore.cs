using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_shared.Models;
using chldr_utils;
using chldr_utils.Models;
using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Stores
{
    public class ContentStore
    {

        #region Events
        public event Action DatabaseInitialized;
        public event Action<SearchResultModel> GotNewSearchResult;

        private readonly ExceptionHandler _exceptionHandler;
        #endregion

        #region Fields and Properties
        private readonly IDataAccess _dataAccess;
        // This shouldn't be normally used, but only to request models that have already been loaded 
        public List<SearchResultModel> CachedSearchResults { get; } = new();
        public List<LanguageModel> AllLanguages { get; } = new();
        // These are the sources excluding userIds
        public List<SourceModel> AllNamedSources { get; } = new();
        #endregion

        #region EventHandlers
        private void DataAccess_GotNewSearchResults(SearchResultModel searchResult)
        {
            CachedSearchResults.Add(searchResult);
            GotNewSearchResult?.Invoke(searchResult);
        }
        #endregion

        #region Constructors
        public ContentStore(IDataAccessFactory dataAccessFactory, ExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            _dataAccess = dataAccessFactory.GetInstance(DataAccess.CurrentDataAccess);
            _dataAccess.EntriesRepository.GotNewSearchResult += DataAccess_GotNewSearchResults;
            _dataAccess.DatabaseInitialized += DataAccess_DatabaseInitialized;
            _dataAccess.Initialize();
        }
        #endregion

        #region Methods
        public void Search(string inputText, FiltrationFlags filterationFlags)
        {
            CachedSearchResults.Clear();
            Task.Run(() => _dataAccess.EntriesRepository.FindAsync(inputText, filterationFlags));
        }
        public void LoadRandomEntries()
        {
            CachedSearchResults.Clear();
            _dataAccess.EntriesRepository.RequestRandomEntries();
        }
        public List<EntryModel> GetRandomEntries()
        {
            var randoms = _dataAccess.EntriesRepository.GetRandomEntries();
            return randoms;
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


        private void DataAccess_DatabaseInitialized()
        {
            try
            {
                var languages = _dataAccess.GetAllLanguages();
                AllLanguages.AddRange(languages);

                var namedSources = _dataAccess.GetAllNamedSources();
                AllNamedSources.AddRange(namedSources);

                DatabaseInitialized?.Invoke();
            }
            catch (Exception ex)
            {
                _exceptionHandler.ProcessError(ex);
            }
        }

        public void Search(string query)
        {
            Search(query, new FiltrationFlags());
        }

        public void LoadEntriesOnModeration()
        {
            CachedSearchResults.Clear();
            Task.Run(() => _dataAccess.EntriesRepository.RequestEntriesOnModeration());
        }

        public PhraseModel AddNewPhrase(UserModel userModel, string content, string notes)
        {
            PhraseModel phrase = _dataAccess.PhrasesRepository.Add(content, notes);
            return phrase;
        }


        public PhraseModel GetCachedPhraseById(ObjectId phraseId)
        {
            // Get current Phrase from cached results
            var phrase = CachedSearchResults.SelectMany(sr => sr.Entries)
                .Where(e => (EntryType)e.Type == EntryType.Phrase)
                .Cast<PhraseModel>()
                .FirstOrDefault(w => w.Id == phraseId);

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
    }
}