using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Realms;
using Realms.Logging;
using Realms.Sync;
using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
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

        private string KeyAsString()
        {
            byte[] encryptionKey = File.ReadAllBytes(Path.Combine(_fileService.AppDataDirectory, "encryption.key"));

            var key = encryptionKey.Select(b => (int)b);
            var stringified = string.Join(":", key);
            return stringified;
        }

        public void InitializeConfiguration()
        {
            // Copy original file so that app will be able to access entries immediately
            byte[] encKey = AppConstants.EncKey.Split(":").Select(numAsString => Convert.ToByte(numAsString)).ToArray();

            var encryptedConfig = new RealmConfiguration(_fileService.OfflineDatabaseFilePath)
            {
                SchemaVersion = 1,
                EncryptionKey = encKey
            };

            _config = encryptedConfig;
        }
    }
}