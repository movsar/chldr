using Data.Entities;
using Realms;
using System.Collections.Immutable;
using System.Diagnostics;
using MongoDB.Bson;
using Data.Models;
using Data.Search;
using Data.Enums;
using Data.Interfaces;
using Data.Services.PartialMethods;
using Realms.Sync;
using Realms.Sync.ErrorHandling;
using Realms.Sync.Exceptions;
using Realms.Logging;

namespace Data.Services
{
    public class RealmDataAccessService : IDataAccessService
    {
        public const int ResultsLimit = 100;
        public const int RandomEntriesLimit = 30;

        public Action<string, SearchResultsModel> NewSearchResults;

        private App _app;
        private User _user;
        private Realm _database;

        private void AddSubscriptionsToRealm()
        {
            var subscriptions = _database.Subscriptions;
            subscriptions.Update(() =>
            {
                subscriptions.Add(_database.All<EntryEntity>());
                subscriptions.Add(_database.All<WordEntity>());
                subscriptions.Add(_database.All<TextEntity>());
                subscriptions.Add(_database.All<PhraseEntity>());
                subscriptions.Add(_database.All<SourceEntity>());
                subscriptions.Add(_database.All<LanguageEntity>());
                subscriptions.Add(_database.All<UserEntity>());
            });
        }
        private async Task ConnectToSyncedDatabase()
        {
            Logger.LogLevel = LogLevel.Debug;
            // customize the logging function:
            Logger.Default = Logger.Function(message =>
            {
                Debug.WriteLine($"Realm Logs: {message}");
            });

            var myRealmAppId = "dosham-ahtcj";
            _app = App.Create(myRealmAppId);
            _user = await _app.LogInAsync(Credentials.Anonymous());
            _database = GetRealmInstance();

            //AddSubscriptionsToRealm();
            await _database.Subscriptions.WaitForSynchronizationAsync();
        }
        public async Task DoDangerousTheStuff()
        {
            // Do something crazy
        }

        public RealmDataAccessService()
        {
            var fs = new FileService();
            fs.PrepareDatabase();

            Task.Run(async () =>
            {
                await ConnectToSyncedDatabase();
                await DoDangerousTheStuff();
            }).Wait();
        }

        public async Task<IEnumerable<EntryModel>> GetRandomEntries()
        {
            var randomizer = new Random();
            var entries = GetRealmInstance().All<EntryEntity>().AsEnumerable();

            // Takes random entries and shuffles them to break the natural order
            return entries
                .Where(entry => entry.Rate > 0)
                .OrderBy(x => randomizer.Next(0, 70000))
                .Take(RandomEntriesLimit)
                .OrderBy(entry => entry.GetHashCode())
                .Select(entry => new EntryModel(entry));
        }

        public async Task FindAsync(string inputText)
        {
            var searchEngine = new MainSearchEngine(this);
            await searchEngine.FindAsync(inputText);
        }
        internal Realm GetRealmInstance()
        {
            var config = new FlexibleSyncConfiguration(_user, Path.Combine(FileService.AppDataDirectory, FileService.DatabaseName))
            {
                PopulateInitialSubscriptions = (realm) =>
                {
                    realm.Subscriptions.Add(_database.All<EntryEntity>());
                    realm.Subscriptions.Add(_database.All<WordEntity>());
                    realm.Subscriptions.Add(_database.All<TextEntity>());
                    realm.Subscriptions.Add(_database.All<PhraseEntity>());
                    realm.Subscriptions.Add(_database.All<SourceEntity>());
                    realm.Subscriptions.Add(_database.All<LanguageEntity>());
                    realm.Subscriptions.Add(_database.All<UserEntity>());
                }
            };
            return Realm.GetInstance(config);
        }
    }
}
