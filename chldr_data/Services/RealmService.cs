using chldr_data.Entities;
using chldr_data.Interfaces;
using Realms;
using Realms.Logging;
using Realms.Sync;
using System;
using System.Diagnostics;
using System.IO.Compression;
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

        // Don't touch this unless it's absolutely necessary! It was very hard to configure!
        internal Realm GetRealm()
        {
            if (_config == null)
            {
                throw new Exception("Config shouldn't be null");
            }

            return Realm.GetInstance(_config);
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

            var appUser = _app.CurrentUser;

            // Log in as anonymous user to be able to read data
            if (appUser?.State != UserState.LoggedIn)
            {
                appUser = await _app.LogInAsync(Credentials.Anonymous(true));
            }
        }

        internal void UpdateRealmConfig(Realms.Sync.User appUser)
        {
            if (appUser == null)
            {
                throw new Exception("User must not be null");
            }

            var uncompressedFileName = FileService.CompressedDatabaseFileName + "x";
            var userDatabaseName = FileService.GetUserDatabaseName(appUser.Id);
            var userDatabasePath = Path.Combine(FileService.AppDataDirectory, userDatabaseName);

            if (!File.Exists(userDatabasePath) && !File.Exists(uncompressedFileName))
            {
                ZipFile.ExtractToDirectory(FileService.CompressedDatabaseFilePath, FileService.AppDataDirectory);
                File.Move(Path.Combine(FileService.AppDataDirectory, uncompressedFileName), Path.Combine(FileService.AppDataDirectory, userDatabaseName));
            }

            _config = new FlexibleSyncConfiguration(appUser, userDatabasePath)
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

            // To wait for syncrhonization, use realm.SyncSession.WaitForDownload(),
            // not Subscriptions.WaitForSynchronization()

            // Get database size in megabytes
            var fileSize = new FileInfo(userDatabasePath).Length / 1000_000;
            if (fileSize > 50)
            {
                Realm.Compact(_config);
            }

            DatabaseInitialized?.Invoke();
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