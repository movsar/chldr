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
using Entry = Data.Entities.Entry;
using User = Data.Entities.User;

namespace Data.Services
{
    public class OnlineDataAccess : DataAccess
    {
        private static App _app;
        private static Realms.Sync.User _user;
        private static FlexibleSyncConfiguration _config;
        private static Realm _realm;

        private async Task ConnectToSyncedDatabase()
        {
            Logger.LogLevel = LogLevel.Debug;
            Logger.Default = Logger.Function(message =>
            {
                Debug.WriteLine($"APP: Realm : {message}");
            });

            var myRealmAppId = "dosham-lxwuu";

            if (!Directory.Exists(FileService.AppDataDirectory))
            {
                Directory.CreateDirectory(FileService.AppDataDirectory);
            }

            _app = App.Create(new AppConfiguration(myRealmAppId)
            {
                BaseFilePath = FileService.AppDataDirectory,
            });

            _user = await _app.LogInAsync(Credentials.Anonymous());

            _config = new FlexibleSyncConfiguration(_user, Path.Combine(FileService.AppDataDirectory, FileService.DatabaseName))
            {
                PopulateInitialSubscriptions = (realm) =>
                {
                    Debug.WriteLine($"APP: Realm : PopulateInitialSubscriptions");

                    realm.Subscriptions.Add(realm.All<Entry>());
                    realm.Subscriptions.Add(realm.All<Language>());
                    realm.Subscriptions.Add(realm.All<Phrase>());
                    realm.Subscriptions.Add(realm.All<Source>());
                    realm.Subscriptions.Add(realm.All<Translation>());
                    realm.Subscriptions.Add(realm.All<User>());
                    realm.Subscriptions.Add(realm.All<Word>());
                }
            };
            _realm = Realm.GetInstance(_config);
            await _realm.Subscriptions.WaitForSynchronizationAsync();
            //_realm.WriteCopy(new RealmConfiguration(Path.Combine(FileService.AppDataDirectory, "compact.realm")));
        }
        public OnlineDataAccess()
        {
            var fs = new FileService();
            fs.PrepareDatabase();
            ConnectToSyncedDatabase().Wait();
        }

        internal override Realm GetRealmInstance()
        {
            return Realm.GetInstance(_config);
        }

        public async Task RegisterNewUser(string email, string password, string username, string firstName, string lastName)
        {
            await _app.EmailPasswordAuth.RegisterUserAsync(email, password);
        }

        public async Task ConfirmUser(string token, string tokenId)
        {
            await _app.EmailPasswordAuth.ConfirmUserAsync(token, tokenId);
        }
    }
}