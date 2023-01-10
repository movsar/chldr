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
        private FlexibleSyncConfiguration _config;

        private void AddSubscriptionsToRealm(Realm instance)
        {
            var subscriptions = instance.Subscriptions;
            subscriptions.Update(() =>
            {
                subscriptions.Add(instance.All<EntryEntity>());
                subscriptions.Add(instance.All<WordEntity>());
                subscriptions.Add(instance.All<TextEntity>());
                subscriptions.Add(instance.All<PhraseEntity>());
                subscriptions.Add(instance.All<SourceEntity>());
                subscriptions.Add(instance.All<LanguageEntity>());
                subscriptions.Add(instance.All<UserEntity>());
            });
        }

        private async Task ConnectToSyncedDatabase()
        {
            Logger.LogLevel = LogLevel.Debug;
            Logger.Default = Logger.Function(message =>
            {
                Debug.WriteLine($"APP: Realm : {message}");
            });

            var myRealmAppId = "dosham-ahtcj";
            _app = App.Create(myRealmAppId);
            _user = await _app.LogInAsync(Credentials.Anonymous());

            _config = new FlexibleSyncConfiguration(_user, Path.Combine(FileService.AppDataDirectory, FileService.DatabaseName));
            var realm = Realm.GetInstance(_config);
            AddSubscriptionsToRealm(realm);
        }

        public async Task DoDangerousTheStuff()
        {
            // Do something crazy
        }

        public RealmDataAccessService()
        {
            var fs = new FileService();
            fs.PrepareDatabase();
        }

        public IEnumerable<EntryModel> GetRandomEntries()
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

        public async Task WaitForSync()
        {
            await GetRealmInstance().Subscriptions.WaitForSynchronizationAsync();
        }

        internal Realm GetRealmInstance()
        {
            if (_app == null)
            {
                ConnectToSyncedDatabase().Wait();
            }
            
            return Realm.GetInstance(_config);
        }
    }
}
