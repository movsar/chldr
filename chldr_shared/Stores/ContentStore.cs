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
        public event Action DatabaseInitialized;

        #region Events
        public event Action CurrentEntriesUpdated;
        #endregion

        #region Fields
        private readonly IDataAccess _dataAccess;
        #endregion

        #region Constructors
        public ContentStore(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _dataAccess.GotResults += DataAccess_GotNewResults;
        
            _dataAccess.DatabaseInitialized += (() =>
            {
                DatabaseInitialized?.Invoke(); 
            });
        }

        #endregion

        #region Properties
        public List<EntryModel> CurrentEntries { get; } = new();
        #endregion

        #region EventHandlers
        private void DataAccess_GotNewResults(SearchResultsModel results)
        {
            CurrentEntries.AddRange(results.Entries);
            CurrentEntriesUpdated?.Invoke();
        }
        #endregion

        #region Methods
        public void Search(string inputText)
        {
            CurrentEntries.Clear();
            Task.Run(() => _dataAccess.FindAsync(inputText));
        }

        public void LoadRandomEntries()
        {
            var entries = _dataAccess.GetRandomEntries();

            CurrentEntries.Clear();
            CurrentEntries.AddRange(entries);
            CurrentEntriesUpdated?.Invoke();
        }
        public WordModel GetWordById(ObjectId entryId)
        {
            return _dataAccess.GetWordById(entryId);
        }
        public PhraseModel GetPhraseById(ObjectId entryId)
        {
            return _dataAccess.GetPhraseById(entryId);
        }

        public async Task RegisterNewUser(string email, string password)
        {
            await _dataAccess.RegisterNewUserAsync(email, password);
        }

        #endregion

    }
}
