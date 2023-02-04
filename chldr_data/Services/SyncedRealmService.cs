﻿using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using Realms;
using Realms.Logging;
using Realms.Sync;
using Realms.Sync.Exceptions;
using System.Diagnostics;
using static Realms.Sync.MongoClient;

namespace chldr_data.Services
{
    public class SyncedRealmService : IRealmService
    {

        private const string myRealmAppId = "dosham-lxwuu";
        internal event Action? DatabaseSynchronized;
        public event Action? DatasourceInitialized;

        private App? _app;
        private FlexibleSyncConfiguration? _config;

        private readonly ExceptionHandler _exceptionHandler;
        private readonly FileService _fileService;

        public Realm GetDatabase()
        {
            if (_app == null || _config == null)
            {
                throw new Exception("Config shouldn't be null");
            }

            return Realm.GetInstance(_config);
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
        private async Task SynchronizeDatabase()
        {
            try
            {
                await GetDatabase().Subscriptions.WaitForSynchronizationAsync();
                await GetDatabase().SyncSession.WaitForDownloadAsync();
                await GetDatabase().SyncSession.WaitForUploadAsync();

                DatabaseSynchronized?.Invoke();
            }
            catch (Exception ex)
            {
                _exceptionHandler.ProcessDebug(ex, "Error whille synchronizing database");
            }
        }

        private async Task InitializeApp()
        {
            if (_app == null)
            {
                _app = App.Create(new AppConfiguration(myRealmAppId)
                {
                    BaseFilePath = _fileService.AppDataDirectory,
                });
            }
        }

        private string GetUserDatabaseName(string id)
        {
            return $"{id.Substring(4, 4)}.dbx";
        }
        public void InitializeConfiguration()
        {
            // As this is FlexibleSync mode, the user must always be present, even if it's offline

            // Log in as anonymous user to be able to read data
            if (_app?.CurrentUser == null)
            {
                throw new Exception("User must not be null");
            }

            //if (_app.CurrentUser?.State != UserState.LoggedIn)
            //{
            //    throw new Exception("User is not logged in");
            //}

            // Copy original file so that app will be able to access entries immediately
            var syncedDatabasePath = Path.Combine(_fileService.AppDataDirectory, GetUserDatabaseName(_app.CurrentUser.Id));

            byte[] encKey = AppConstants.EncKey.Split(":").Select(numAsString => Convert.ToByte(numAsString)).ToArray();

            _config = new FlexibleSyncConfiguration(_app.CurrentUser, syncedDatabasePath)
            {
                EncryptionKey = encKey,
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
                },
                OnSessionError = (session, sessionException) =>
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
                }
            };

            // Compress the database file if it exceeds 70Mb
            if (File.Exists(syncedDatabasePath))
            {
                var fileSize = new FileInfo(syncedDatabasePath).Length / 1000_000;
                if (fileSize > 70)
                {
                    Realm.Compact(_config);
                }
            }
        }

        public void InitializeDataSource()
        {
            Task.Run(async () =>
            {
                await InitializeApp();
                InitializeConfiguration();

                var language = GetDatabase().All<Language>().FirstOrDefault();
                if (language == null)
                {
                    await SynchronizeDatabase();
                }

                DatasourceInitialized?.Invoke();
            }).Start();
        }
    }
}