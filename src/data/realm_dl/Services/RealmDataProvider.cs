using domain.Interfaces;
using Realms;
using realm_dl.RealmEntities;
using realm_dl.Interfaces;
using domain;

namespace realm_dl.Services
{
    public class RealmDataProvider : IDataProvider
    {
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IFileService _fileService;
        private readonly ISyncService _syncService;
        internal static RealmConfigurationBase? OfflineDatabaseConfiguration;

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
            IFileService fileService,
            IExceptionHandler exceptionHandler,
            ISyncService syncService)
        {
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
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
            var encryptedConfig = new RealmConfiguration(_fileService.DatabaseFilePath)
            {
                EncryptionKey = encKey
            };

            return encryptedConfig;
        }
        public void Initialize()
        {
            OfflineDatabaseConfiguration = new RealmConfiguration(_fileService.DatabaseFilePath)
            {
                SchemaVersion = Constants.RealmSchemaVersion
            };

            DatabaseInitialized?.Invoke();
            //_syncService.BeginListening();
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
        public IDataAccessor Repositories(string? userId)
        {          
            return new RealmDataAccessor(_exceptionHandler, _fileService, userId);
        }

        public Realm GetContext()
        {
            return GetDatabase();
        }
    }
}