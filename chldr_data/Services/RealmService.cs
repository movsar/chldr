using chldr_data.Entities;
using chldr_data.Interfaces;
using Realms;
using Realms.Logging;
using Realms.Sync;
using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace chldr_data.Services
{
    public class RealmService
    {
        private const string myRealmAppId = "dosham-lxwuu";

        private App _app { get; set; }
        private RealmConfigurationBase _config;

        internal event Action DatabaseInitialized;
        internal event Action DatabaseSynced;

        private static Realm _realmInstance;

        // Don't touch this unless it's absolutely necessary! It was very hard to configure!
        internal Realm GetRealm()
        {
            if (_config == null)
            {
                throw new Exception("Config shouldn't be null");
            }

            if (_realmInstance == null)
            {
                return _realmInstance = Realm.GetInstance(_config);
            }

            if (_realmInstance.IsClosed || (_realmInstance.SyncSession?.User != null && _realmInstance.SyncSession?.User != _app.CurrentUser))
            {
                _realmInstance.Dispose();
                return _realmInstance = Realm.GetInstance(_config);
            }

            return _realmInstance = Realm.GetInstance(_config);
        }

        internal async Task InitializeApp()
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
                var appUser = _app.CurrentUser;

                // Log in as anonymous user to be able to read data
                if (appUser?.State != UserState.LoggedIn)
                {
                    appUser = await _app.LogInAsync(Credentials.Anonymous(true));
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            // TODO: Compact if size > 100Mb
            // Realm.Compact(_config);

            //Task.Run(async () =>
            //{
            //    await GetRealm().Subscriptions.WaitForSynchronizationAsync();
            //    DatabaseSynced?.Invoke();
            //});
        }

        internal void InitializeDatabase()
        {
            RefreshRealmConfig(_app.CurrentUser);
            DatabaseInitialized?.Invoke();
        }
        internal void RefreshRealmConfig(Realms.Sync.User appUser)
        {
            _config = GetRealmConfigForSpecifiedUser(appUser);
        }

        private RealmConfigurationBase GetRealmConfigForSpecifiedUser(Realms.Sync.User appUser)
        {
            if (appUser == null)
            {
                throw new Exception("User must not be null");
            }

            return new FlexibleSyncConfiguration(appUser, FileService.DatabasePath)
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

        internal App GetApp()
        {
            return _app;
        }

        private static void EncryptDatbase()
        {
            //using var realm = GetRealm();

            //var encryptionKey = new byte[64];
            //using var rng = new RNGCryptoServiceProvider();
            //rng.GetBytes(encryptionKey);

            //var encryptedConfig = new RealmConfiguration(Path.Combine(FileService.AppDataDirectory, "encrypted.realm"))
            //{
            //    EncryptionKey = encryptionKey
            //};

            //realm.WriteCopy(encryptedConfig);
            //File.WriteAllBytes(Path.Combine(FileService.AppDataDirectory, "encryption.key"), encryptionKey);

        }
    }
}