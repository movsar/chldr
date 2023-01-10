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

namespace Data.Services
{
    public class RealmDataAccessService : IDataAccessService
    {
        public const int ResultsLimit = 100;
        public const int RandomEntriesLimit = 30;

        public Action<string, SearchResultsModel> NewSearchResults;
        private App _app;
        private User _user;

        Realm RealmDatabase = null;
   
        private void AddSubscriptionsToRealm()
        {
            var subscriptions = RealmDatabase.Subscriptions;
            subscriptions.Update(() =>
            {
                subscriptions.Add(RealmDatabase.All<EntryEntity>());
                subscriptions.Add(RealmDatabase.All<WordEntity>());
                subscriptions.Add(RealmDatabase.All<TextEntity>());
                subscriptions.Add(RealmDatabase.All<PhraseEntity>());
                subscriptions.Add(RealmDatabase.All<SourceEntity>());
                subscriptions.Add(RealmDatabase.All<LanguageEntity>());
                subscriptions.Add(RealmDatabase.All<UserEntity>());
            });
        }
    
        private async Task ConnectToSyncedDatabase()
        {
            var myRealmAppId = "dosham-ahtcj";
            _app = App.Create(myRealmAppId);
            _user = await _app.LogInAsync(Credentials.Anonymous());

            RealmDatabase = await GetRealmInstanceAsync();

            AddSubscriptionsToRealm();

            await RealmDatabase.Subscriptions.WaitForSynchronizationAsync();
        }
        public async Task DoDangerousTheStuff()
        {
            //await UpdatePhrasesByChunks();

            //CopyObjectIds();
            //RemoveWeirdos();
            //SetSourceNotes();
            //ImportPhrases();
            //UpdateEntryRawContentField();
            //RemoveDuplicates();
            //SetTranslationEntryAndUserLinks();
            //RemoveExistingDuplicatingInLegacyPhrases();
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
            var entries = (await GetRealmInstanceAsync()).All<EntryEntity>().AsEnumerable();

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

        internal async Task<Realm> GetRealmInstanceAsync()
        {
            var config = new FlexibleSyncConfiguration(_user, Path.Combine(FileService.AppDataDirectory, FileService.DatabaseName));
            return await Realm.GetInstanceAsync(config);
        }
    }
}
