using chldr_data.Entities;
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
    public class OfflineRealmService : IRealmService
    {
        private readonly ExceptionHandler _exceptionHandler;
        private readonly FileService _fileService;
        private RealmConfigurationBase? _config;
        public Realm GetDatabase()
        {
            if (_config == null)
            {
                throw new Exception("Config shouldn't be null");
            }

            return Realm.GetInstance(_config);
        }
        public OfflineRealmService(FileService fileService, ExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
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
        public void InitializeDatabase()
        {
            // Copy original file so that app will be able to access entries immediately
            var databasePath = Path.Combine(_fileService.AppDataDirectory, "offline.datx");
            PrepareOriginalDatabaseFile(databasePath);

            _config = new RealmConfiguration(databasePath)
            {
                SchemaVersion = 1,
            };

            // Compress the database file if it exceeds 70Mb
            if (File.Exists(databasePath))
            {
                var fileSize = new FileInfo(databasePath).Length / 1000_000;
                if (fileSize > 70)
                {
                    Realm.Compact(_config);
                }
            }
        }

    }
}