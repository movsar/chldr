﻿using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using Realms;
using Realms.Logging;
using Realms.Sync;
using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using static Realms.Sync.MongoClient;

namespace chldr_data.Services
{
    public class RealmService
    {
        private const string myRealmAppId = "dosham-lxwuu";
        private readonly ExceptionHandler _exceptionHandler;
        private readonly FileService _fileService;

        private App _app { get; set; }
        private RealmConfigurationBase _config;
        private string userDatabasePath;

        public RealmService(FileService fileService, ExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
        }

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
                Debug.WriteLine($"Realm : {message}");
                // _exceptionHandler.ProcessError(new Exception($"Realm : {message}"));
            });

            _app = App.Create(new AppConfiguration(myRealmAppId)
            {
                BaseFilePath = _fileService.AppDataDirectory,
            });

            var appUser = _app.CurrentUser;

            // Log in as anonymous user to be able to read data
            if (appUser?.State != UserState.LoggedIn)
            {
                appUser = await _app.LogInAsync(Credentials.Anonymous(true));
            }
        }

        internal async void UpdateRealmConfig(Realms.Sync.User appUser)
        {
            if (appUser == null)
            {
                throw new Exception("User must not be null");
            }

            var uncompressedFileName = _fileService.CompressedDatabaseFileName + "x";
            var userDatabaseName = _fileService.GetUserDatabaseName(appUser.Id);
            userDatabasePath = Path.Combine(_fileService.AppDataDirectory, userDatabaseName);

            if (!File.Exists(userDatabasePath) && !File.Exists(uncompressedFileName))
            {
                try
                {
                    ZipFile.ExtractToDirectory(_fileService.CompressedDatabaseFilePath, _fileService.AppDataDirectory);
                    File.Move(Path.Combine(_fileService.AppDataDirectory, uncompressedFileName), Path.Combine(_fileService.AppDataDirectory, userDatabaseName));


                }
                catch (Exception ex)
                {
                    _exceptionHandler.ProcessDebug(ex);
                }
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

            if (File.Exists(userDatabasePath))
            {
                var fileSize = new FileInfo(userDatabasePath).Length / 1000_000;
                if (fileSize > 70)
                {
                    Realm.Compact(_config);
                }
            }
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

            //var encryptedConfig = new RealmConfiguration(Path.Combine(_fileService.AppDataDirectory, "encrypted.realm"))
            //{
            //    EncryptionKey = encryptionKey
            //};

            //realm.WriteCopy(encryptedConfig);
            //File.WriteAllBytes(Path.Combine(_fileService.AppDataDirectory, "encryption.key"), encryptionKey);

        }
    }
}