using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_utils;
using chldr_utils.Services;
using Realms;
using Realms.Sync;
using System.Security.Cryptography;

namespace chldr_data.Services
{
    public abstract class DataAccess : IDataAccess
    {
        public event Action? DatabaseInitialized;

        protected readonly ExceptionHandler _exceptionHandler;
        protected readonly NetworkService _networkService;
        protected readonly IRealmServiceFactory _realmServiceFactory;
        public static DataAccessType CurrentDataAccess { get; set; } = DataAccessType.Offline;
        public Realm Database => _realmServiceFactory.GetInstance().GetDatabase();
        public SourcesRepository SourcesRepository { get; }
        public LanguagesRepository LanguagesRepository { get; }
        public WordsRepository WordsRepository { get; }
        public PhrasesRepository PhrasesRepository { get; }
        public EntriesRepository<EntryModel> EntriesRepository { get; }


        public DataAccess(IRealmServiceFactory realmServiceFactory, ExceptionHandler exceptionHandler, NetworkService networkService)
        {
            // Must be run on a different thread, otherwise the main view will not be able to listen to its events

            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _realmServiceFactory = realmServiceFactory;

            EntriesRepository = new EntriesRepository<EntryModel>(_realmServiceFactory);
            WordsRepository = new WordsRepository(_realmServiceFactory);
            PhrasesRepository = new PhrasesRepository(_realmServiceFactory);
            LanguagesRepository = new LanguagesRepository(_realmServiceFactory);
            SourcesRepository = new SourcesRepository(_realmServiceFactory);
        }

        private void RealmService_DatasourceInitialized()
        {
            DatabaseInitialized?.Invoke();
        }

        public void Initialize()
        {
            try
            {
                var realmService = _realmServiceFactory.GetInstance();
                realmService.DatasourceInitialized += RealmService_DatasourceInitialized;
                realmService.InitializeDataSource();

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("998"))
                {
                    // Network error
                    _exceptionHandler.ProcessError(new Exception("NETWORK_ERROR"));
                }
                else
                {
                    _exceptionHandler.ProcessError(ex);
                }
            }
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

        public async Task DatabaseMaintenance()
        {
            await Database.SyncSession.WaitForDownloadAsync();
            await Database.SyncSession.WaitForUploadAsync();

            string myTestRealmAppId = "dosham-test-oaqel";
            var app = App.Create(new AppConfiguration(myTestRealmAppId));

            try
            {
                var sources = Database.All<Source>().ToList();
                var unverifiedSources = SourcesRepository.GetUnverifiedSources();

                Database.Write(() =>
                {

                });

            }
            catch (Exception ex)
            {
                _exceptionHandler.ProcessError(ex);
            }
        }

    }
}
