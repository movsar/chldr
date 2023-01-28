using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
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
        public ContentStore(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _dataAccess.GotNewSearchResult += DataAccess_GotNewSearchResults;
            _dataAccess.DatabaseInitialized += DataAccess_DatabaseInitialized;
        }
        #endregion

        #region Methods
        public void Search(string inputText)
        {
            CachedSearchResults.Clear();
            Task.Run(() => _dataAccess.FindAsync(inputText));
        }
        public void LoadRandomEntries()
        {
            CachedSearchResults.Clear();
            Task.Run(() => _dataAccess.LoadRandomEntries());
        }
        public WordModel GetWordById(ObjectId entryId)
        {
            return _dataAccess.GetWordById(entryId);
        }
        public PhraseModel GetPhraseById(ObjectId entryId)
        {
            return _dataAccess.GetPhraseById(entryId);
        }

        public EntryModel GetEntryById(ObjectId entryId)
        {
            var word = _dataAccess.GetWordByEntryId(entryId);
            if (word != null)
            {
                return word;
            }

            var phrase = _dataAccess.GetPhraseByEntryId(entryId);
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
            }catch(Exception ex)
            {
                throw ;
            }
            
        }
    }
}   