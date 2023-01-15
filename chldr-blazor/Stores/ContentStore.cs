using Data.Models;
using Data.Services;
using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_blazor.Stores
{
    public class ContentStore
    {

        #region Events
        public event Action CurrentEntriesUpdated;
        #endregion

        #region Fields
        private readonly DataAccess _dataAccess;
        #endregion

        #region Constructors
        public ContentStore(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _dataAccess.GotResults += DataAccess_GotNewResults;
            // _dataAccess.DatabaseInitialized += (() => { });
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
        internal void Search(string inputText)
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

        internal EntryModel GetEntryById(ObjectId entryId)
        {
            return CurrentEntries.FirstOrDefault(e => e.EntryId == entryId);
        }
        #endregion

    }
}
