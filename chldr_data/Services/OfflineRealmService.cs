using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using Realms;
using static Realms.Sync.MongoClient;

namespace chldr_data.Services
{
    public class OfflineRealmService : IRealmService
    {
        private readonly ExceptionHandler _exceptionHandler;
        private readonly FileService _fileService;
        private RealmConfigurationBase? _config;

        public event Action<DataSourceType>? DatasourceInitialized;

        public Realm GetDatabase()
        {
            if (_config == null)
            {
                throw new Exception("Config shouldn't be null");
            }

            Console.WriteLine("in GetDatabase()");
            Console.WriteLine(_config.DatabasePath);

            return Realm.GetInstance(_config);
        }
     
        public OfflineRealmService(FileService fileService, ExceptionHandler exceptionHandler)
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
        public void InitializeConfiguration()
        {
            _config = new RealmConfiguration(_fileService.OfflineDatabaseFilePath);
        }

        public void Initialize()
        {
            InitializeConfiguration();
            DatasourceInitialized?.Invoke(DataSourceType.Offline);
        }

        internal void RemoveAllEntries()
        {
            var database = GetDatabase();
            database.Write(() =>
            {
                database.RemoveAll<Entry>();
                database.RemoveAll<Text>();
                database.RemoveAll<Word>();
                database.RemoveAll<Phrase>();
                database.RemoveAll<Translation>();
            });
        }
    }
}