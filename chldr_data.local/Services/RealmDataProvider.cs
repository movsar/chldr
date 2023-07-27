using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_utils;
using chldr_utils.Services;
using Realms;
using chldr_data.local.RealmEntities;
using chldr_data.local.Services;
using chldr_utils.Interfaces;
using chldr_data.Services;

namespace chldr_data.local.Services
{
    public class RealmDataProvider : IDataProvider
    {
        private readonly ExceptionHandler _exceptionHandler;
        private readonly FileService _fileService;
        private readonly SyncService _syncService;
        private readonly RequestService _requestService;
        internal static RealmConfigurationBase? OfflineDatabaseConfiguration;

        private static string _actingUserId;
        public string ActingUserId => _actingUserId;

        public bool IsInitialized { get; set; }

        public event Action? DatabaseInitialized;

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

        public RealmDataProvider(
            FileService fileService,
            ExceptionHandler exceptionHandler,
            RequestService requestService,
            SyncService syncService
            )
        {
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
            _requestService = requestService;
            _syncService = syncService;
        }

        private string KeyAsString()
        {
            byte[] encryptionKey = File.ReadAllBytes(Path.Combine(_fileService.AppDataDirectory, "encryption.key"));

            var key = encryptionKey.Select(b => (int)b);
            var stringified = string.Join(":", key);
            return stringified;
        }
        private RealmConfiguration GetEncryptedCOnfig()
        {
            // Copy original file so that app will be able to access entries immediately
            byte[] encKey = Constants.EncKey.Split(":").Select(numAsString => Convert.ToByte(numAsString)).ToArray();
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
                SchemaVersion = Constants.RealmSchemaVersion
            };

            DatabaseInitialized?.Invoke();
            _syncService.BeginListening();
        }

        public void TruncateDatabase()
        {
            var database = GetDatabase();
            database.Write(() =>
            {
                database.RemoveAll<RealmEntry>();
                database.RemoveAll<RealmTranslation>();
            });
        }
        public IUnitOfWork CreateUnitOfWork(string? userId = null)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                _actingUserId = userId;
            }
            else if (string.IsNullOrEmpty(_actingUserId))
            {
                _actingUserId = GetContext().All<RealmUser>().First().UserId;
            }

            return new RealmUnitOfWork(_exceptionHandler, _fileService, _requestService, _actingUserId);
        }

        public Realm GetContext()
        {
            return GetDatabase();
        }
    }
}