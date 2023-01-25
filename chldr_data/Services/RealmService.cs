using chldr_data.Entities;
using Realms;
using Realms.Logging;
using Realms.Sync;
using System;
using System.Diagnostics;

namespace chldr_data.Services
{
    internal static class RealmService
    {
        private static App _app;
        private static RealmConfigurationBase _config;
        private const string myRealmAppId = "dosham-lxwuu";

        internal static event Action DatabaseInitialized;
        internal static event Action DatabaseSynced;

        internal static Realm GetRealm()
        {
            if (_config == null)
            {
                throw new Exception("Config shouldn't be null");
            }
            return Realm.GetInstance(_config);
        }

        internal static async Task Initialize()
        {
            Logger.LogLevel = LogLevel.Debug;
            Logger.Default = Logger.Function(message =>
            {
                Debug.WriteLine($"APP: Realm : {message}");
            });

            _app = App.Create(new AppConfiguration(myRealmAppId)
            {
                BaseFilePath = FileService.AppDataDirectory,
            });

            try
            {
                if (_app.CurrentUser?.State != UserState.LoggedIn)
                {
                    await _app.LogInAsync(Credentials.Anonymous());
                }

                _config = new FlexibleSyncConfiguration(_app.CurrentUser, FileService.DatabasePath)
                {
                    SchemaVersion = 1,

                    PopulateInitialSubscriptions = (realm) =>
                    {
                        Debug.WriteLine($"APP: Realm : PopulateInitialSubscriptions");

                        realm.Subscriptions.Add(realm.All<Entities.Entry>());
                        realm.Subscriptions.Add(realm.All<Entities.Language>());
                        realm.Subscriptions.Add(realm.All<Entities.Phrase>());
                        realm.Subscriptions.Add(realm.All<Entities.Source>());
                        realm.Subscriptions.Add(realm.All<Entities.Translation>());
                        realm.Subscriptions.Add(realm.All<Entities.User>());
                        realm.Subscriptions.Add(realm.All<Entities.Word>());
                    }
                };
            }
            catch (Exception ex)
            {
                throw;
            }

            // TODO: Compact if size > 100Mb
            // Realm.Compact(_config);
            DatabaseInitialized?.Invoke();
            //DatabaseSynced?.Invoke();
        }

        internal static App GetApp()
        {
            return _app;
        }
    }
}
