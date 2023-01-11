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
    public class OnlineDataAccess : DataAccess
    {
        private App _app;
        private User _user;
        private FlexibleSyncConfiguration _config;
        private Realm _realm;

        private void AddSubscriptions()
        {
            _realm.Subscriptions.Update(() =>
            {
                _realm.Subscriptions.Add(_realm.All<EntryEntity>());
                _realm.Subscriptions.Add(_realm.All<LanguageEntity>());
                _realm.Subscriptions.Add(_realm.All<PhraseEntity>());
                _realm.Subscriptions.Add(_realm.All<SourceEntity>());
                _realm.Subscriptions.Add(_realm.All<TranslationEntity>());
                _realm.Subscriptions.Add(_realm.All<UserEntity>());
                _realm.Subscriptions.Add(_realm.All<WordEntity>());
            });
        }
        private async Task ConnectToSyncedDatabase()
        {
            Logger.LogLevel = LogLevel.Debug;
            Logger.Default = Logger.Function(message =>
            {
                Debug.WriteLine($"APP: Realm : {message}");
            });

            var myRealmAppId = "chldr-maui-data-jicpt";

            _app = App.Create(myRealmAppId);
            _user = await _app.LogInAsync(Credentials.Anonymous());
            _config = new FlexibleSyncConfiguration(_user, Path.Combine(FileService.AppDataDirectory, FileService.DatabaseName));
            _realm = GetRealmInstance();
            AddSubscriptions();
        }
        public OnlineDataAccess()
        {
            var fs = new FileService();
            fs.PrepareDatabase();
        }

        internal override Realm GetRealmInstance()
        {
            if (_realm == null)
            {
                _realm = Realm.GetInstance(_config);
                return _realm;
            }

            try
            {
                // Check if reference is valid
                _realm.All<LanguageEntity>().ToList();
            }
            catch
            {
                _realm = Realm.GetInstance(_config);
            }

            return _realm;
        }
    }
}