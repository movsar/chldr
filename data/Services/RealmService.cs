using Data.Services.PartialMethods;
using Realms;
using Realms.Logging;
using Realms.Sync;
using System;
using System.Diagnostics;

namespace Data.Services
{
    internal static class RealmService
    {
        private static App _app;
        private static User _user;
        private static FlexibleSyncConfiguration _config;
        private const string myRealmAppId = "dosham-lxwuu";

        internal static event Action DatabaseInitialized;
        internal static event Action DatabaseSynced;

        internal static Realm GetRealm()
        {
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
                // _user = await _app.LogInAsync(Credentials.Anonymous());
                _user = await _app.LogInAsync(Credentials.EmailPassword("movsar.dev@gmail.com", "135790!s-"));
            }
            catch (Exception ex)
            {
                throw;
            }

            _config = new FlexibleSyncConfiguration(_user, Path.Combine(FileService.AppDataDirectory, FileService.DatabaseName))
            {
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

            await GetRealm().Subscriptions.WaitForSynchronizationAsync();
            DatabaseInitialized?.Invoke();
            //DatabaseSynced?.Invoke();
        }

        internal static App GetApp()
        {
            return _app;
        }
    }
}
