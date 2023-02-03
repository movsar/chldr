using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using Realms;
using Realms.Logging;
using Realms.Sync;
using Realms.Sync.Exceptions;
using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using static Realms.Sync.MongoClient;

namespace chldr_data.Services
{
    public class SyncedRealmService : IRealmService
    {
        private const string myRealmAppId = "dosham-lxwuu";

        private App? _app;
        private FlexibleSyncConfiguration? _config;
        private Realm _realmInstance;

        private readonly ExceptionHandler _exceptionHandler;
        private readonly FileService _fileService;

        public Realm GetDatabase()
        {
            if (_app == null || _config == null)
            {
                throw new Exception("Config shouldn't be null");
            }

            _realmInstance = Realm.GetInstance(_config);
            if (_app.CurrentUser.Provider == Credentials.AuthProvider.Anonymous)
            {
                // Don't try to sync anything if a legitimate user hasn't logged in
                //realm.SyncSession.Stop();
            }

            return _realmInstance;
        }
        internal App GetApp()
        {
            return _app;
        }
        public SyncedRealmService(FileService fileService, ExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;

            Logger.LogLevel = LogLevel.Debug;
            Logger.Default = Logger.Function(message =>
            {
                Debug.WriteLine($"Realm : {message}");
                _exceptionHandler.ProcessDebug(new Exception($"Realm : {message}"));
            });
        }

        internal async Task InitializeApp()
        {
            if (_app == null)
            {
                _app = App.Create(new AppConfiguration(myRealmAppId)
                {
                    BaseFilePath = _fileService.AppDataDirectory,
                });
            }

            // Log in as anonymous user to be able to read data
            if (_app.CurrentUser?.State != UserState.LoggedIn)
            {
                await _app.LogInAsync(Credentials.Anonymous(true));
            }
        }

        private void PrepareOriginalDatabaseFile(string targetDatabasePath)
        {
            if (File.Exists(targetDatabasePath))
            {
                return;
            }

            var shippedDbFileName = _fileService.CompressedDatabaseFileName + "x";
            if (!File.Exists(targetDatabasePath) && !File.Exists(shippedDbFileName))
            {
                try
                {
                    ZipFile.ExtractToDirectory(_fileService.CompressedDatabaseFilePath, _fileService.AppDataDirectory);
                    File.Move(Path.Combine(_fileService.AppDataDirectory, shippedDbFileName), targetDatabasePath);
                }
                catch (Exception ex)
                {
                    // This exception is not critical, it will only request the database from the server
                    _exceptionHandler.ProcessDebug(ex);
                }
            }
        }
        public void InitializeConnection()
        {
            // As this is FlexibleSync mode, the user must always be present, even if it's offline
            if (_app.CurrentUser == null)
            {
                throw new Exception("User must not be null");
            }

            // Copy original file so that app will be able to access entries immediately
            var userDatabasePath = Path.Combine(_fileService.AppDataDirectory, _fileService.GetUserDatabaseName(_app.CurrentUser.Id));
            PrepareOriginalDatabaseFile(userDatabasePath);
            _config = new FlexibleSyncConfiguration(_app.CurrentUser, userDatabasePath)
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
            _config.OnSessionError = (session, sessionException) =>
            {
                switch (sessionException.ErrorCode)
                {
                    case ErrorCode.InvalidCredentials:
                        // Tell the user they don't have permissions to work with that Realm
                        Debug.WriteLine("Invalid credentials Error");
                        _exceptionHandler.ProcessDebug(new Exception("Invalid Credentials"));
                        break;
                    case ErrorCode.Unknown:
                        // See https://www.mongodb.com/docs/realm-sdks/dotnet
                        // /latest/reference/Realms.Sync.Exceptions.ErrorCode.html
                        // for all of the error codes
                        _exceptionHandler.ProcessDebug(new Exception("Unknown Sync Error"));
                        break;
                }
            };

            // Compress the database file if it exceeds 70Mb
            if (File.Exists(userDatabasePath))
            {
                var fileSize = new FileInfo(userDatabasePath).Length / 1000_000;
                if (fileSize > 70)
                {
                    Realm.Compact(_config);
                }
            }

            EncryptDatbase();
        }

        private void EncryptDatbase()
        {
            //using var realm = GetDatabase();

            //var encryptionKey = new byte[64];
            //using var rng = new RNGCryptoServiceProvider();
            //rng.GetBytes(encryptionKey);

            //var encryptedConfig = new RealmConfiguration(Path.Combine(_fileService.AppDataDirectory, "encrypted.realm"))
            //{
            //    EncryptionKey = encryptionKey
            //};

            //realm.WriteCopy(encryptedConfig);
            //File.WriteAllBytes(Path.Combine(_fileService.AppDataDirectory, "encryption.key"), encryptionKey);
            //Debug.WriteLine("Encryped");
        }
    }
}