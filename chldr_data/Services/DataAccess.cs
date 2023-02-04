using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_utils;
using chldr_utils.Services;
using Realms;
using System.Security.Cryptography;

namespace chldr_data.Services
{
    public abstract class DataAccess : IDataAccess
    {
        protected readonly ExceptionHandler _exceptionHandler;
        protected readonly NetworkService _networkService;
        protected readonly IRealmService _realmService;

        public static DataAccessType CurrentDataAccess { get; set; } = DataAccessType.Synced;

        public abstract Realm Database { get; }
        public LanguagesRepository LanguagesRepository { get; }
        public WordsRepository WordsRepository { get; }
        public PhrasesRepository PhrasesRepository { get; }
        public EntriesRepository<EntryModel> EntriesRepository { get; }

        public event Action? DatabaseInitialized;

        public DataAccess(IRealmService realmService, ExceptionHandler exceptionHandler, NetworkService networkService)
        {
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _realmService = realmService;

            EntriesRepository = new EntriesRepository<EntryModel>(_realmService);
            WordsRepository = new WordsRepository(_realmService);
            PhrasesRepository = new PhrasesRepository(_realmService);
            LanguagesRepository = new LanguagesRepository(_realmService);
        }

        public void OnDatabaseInitialized()
        {
            DatabaseInitialized?.Invoke();
        }
        public List<LanguageModel> GetAllLanguages()
        {
            try
            {
                var languages = Database.All<Language>().AsEnumerable().Select(l => new LanguageModel(l));
                return languages.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SourceModel> GetAllNamedSources()
        {
            return Database.All<Source>().AsEnumerable().Select(s => new SourceModel(s)).ToList();
        }

        public abstract void Initialize();

        private void EncryptDatbase(string path)
        {
            using var realm = Database;

            var encryptionKey = new byte[64];
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(encryptionKey);

            var encryptedConfig = new RealmConfiguration(path)
            {
                EncryptionKey = encryptionKey
            };

            realm.WriteCopy(encryptedConfig);

            // Save key
            File.WriteAllBytes("encryption.key", encryptionKey);
        }

    }
}
