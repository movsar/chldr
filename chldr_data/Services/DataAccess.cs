﻿using chldr_data.Entities;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Search;
using chldr_data.Services.PartialMethods;
using MongoDB.Bson;
using Realms.Sync;
using Entry = chldr_data.Entities.Entry;

namespace chldr_data.Services
{
    public class DataAccess : IDataAccess
    {
        #region Properties
        public Realms.Sync.App App => RealmService.GetApp();
        #endregion

        #region Events
        public event Action DatabaseInitialized;
        public event Action<SearchResultsModel> GotResults;
        #endregion

        #region Fields
        public const int ResultsLimit = 100;
        public const int RandomEntriesLimit = 30;
        #endregion

        public async Task Login(string email, string password)
        {
            await App.LogInAsync(Credentials.EmailPassword(email, password));
        }
        public async Task InitializeDatabase()
        {
            await RealmService.Initialize();
        }
        public async Task FindAsync(string inputText)
        {
            var searchEngine = new MainSearchEngine(this);
            await searchEngine.FindAsync(inputText);
        }
        public IEnumerable<EntryModel> GetRandomEntries()
        {
            var randomizer = new Random();
            var entries = RealmService.GetRealm().All<Entry>().AsEnumerable();

            // Takes random entries and shuffles them to break the natural order
            return entries
                .Where(entry => entry.Rate > 0)
                .OrderBy(x => randomizer.Next(0, 70000))
                .Take(RandomEntriesLimit)
                .OrderBy(entry => entry.GetHashCode())
                .Select(entry => EntryModelFactory.CreateEntryModel(entry));
        }

        public DataAccess()
        {
            var fileService = new FileService();
            fileService.PrepareDatabase();

            RealmService.DatabaseInitialized += () => { DatabaseInitialized?.Invoke(); };
            Task.Run(() => RealmService.Initialize());
        }

        public void OnNewResults(SearchResultsModel results)
        {
            GotResults?.Invoke(results);
        }

        public async Task RegisterNewUser(string email, string password, string username, string firstName, string lastName)
        {
            await App.EmailPasswordAuth.RegisterUserAsync(email, password);
        }

        public WordModel GetWordById(ObjectId entityId)
        {
            return new WordModel(RealmService.GetRealm().All<Word>().FirstOrDefault(w => w._id == entityId));
        }

        public PhraseModel GetPhraseById(ObjectId entityId)
        {
            return new PhraseModel(RealmService.GetRealm().All<Phrase>().FirstOrDefault(p => p._id == entityId));
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await App.EmailPasswordAuth.CallResetPasswordFunctionAsync(email, "");
        }
    }
}
