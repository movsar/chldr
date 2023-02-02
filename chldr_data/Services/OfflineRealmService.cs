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

        public void InitializeDatabase()
        {
            // Copy original file so that app will be able to access entries immediately
            var databasePath = Path.Combine(_fileService.AppDataDirectory, "offline.datx");

            _config = new RealmConfiguration(databasePath);
        }

    }
}