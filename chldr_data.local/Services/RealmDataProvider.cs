using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_utils;
using chldr_utils.Services;
using Realms;
using chldr_data.local.RealmEntities;
using chldr_data.local.Services;

namespace chldr_data.local.Services
{
    public class RealmDataProvider : IDataProvider
    {
        private readonly ExceptionHandler _exceptionHandler;
        private readonly FileService _fileService;
        internal static RealmConfigurationBase? OfflineDatabaseConfiguration;

        public bool IsInitialized { get; set; }

        public event Action? LocalDatabaseInitialized;

        internal Realm GetDatabase()
        {
            if (OfflineDatabaseConfiguration == null)
            {
                throw new Exception("Config shouldn't be null");
            }

            Console.WriteLine("in GetDatabase()");
            Console.WriteLine(OfflineDatabaseConfiguration.DatabasePath);

            return Realm.GetInstance(OfflineDatabaseConfiguration);
        }

        public RealmDataProvider(FileService fileService, ExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
        }

        private string KeyAsString()
        {
            byte[] encryptionKey = File.ReadAllBytes(Path.Combine(FileService.AppDataDirectory, "encryption.key"));

            var key = encryptionKey.Select(b => (int)b);
            var stringified = string.Join(":", key);
            return stringified;
        }
        private RealmConfiguration GetEncryptedCOnfig()
        {
            // Copy original file so that app will be able to access entries immediately
            byte[] encKey = AppConstants.EncKey.Split(":").Select(numAsString => Convert.ToByte(numAsString)).ToArray();
            var hexKey = BitConverter.ToString(encKey).Replace("-", "");
            var encryptedConfig = new RealmConfiguration(_fileService.OfflineDatabaseFilePath)
            {
                EncryptionKey = encKey
            };

            return encryptedConfig;
        }
        public void Initialize()
        {
            OfflineDatabaseConfiguration = new RealmConfiguration(_fileService.OfflineDatabaseFilePath)
            {
                SchemaVersion = 8
            };

            var realm = GetDatabase();
            LocalDatabaseInitialized?.Invoke();
        }

        public void PurgeAllData()
        {
            var database = GetDatabase();
            database.Write(() =>
            {
                database.RemoveAll<RealmEntry>();
                database.RemoveAll<RealmText>();
                database.RemoveAll<RealmWord>();
                database.RemoveAll<RealmPhrase>();
                database.RemoveAll<RealmTranslation>();
            });
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return new RealmUnitOfWork(GetDatabase());
        }
    }
}