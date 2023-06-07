using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.Readers;

namespace chldr_data.Services
{
    public class LocalDbReader : ILocalDbReader
    {
        public event Action? DataSourceInitialized;

        protected readonly ExceptionHandler _exceptionHandler;
        protected readonly NetworkService _networkService;
        private readonly EnvironmentService _environmentService;
        private readonly IDataSource _realmDataSource;

        public IGraphQLRequestSender RequestSender { get; set; }
        public WordsReader Words { get; }
        public PhrasesReader Phrases { get; }
        public LanguagesReader Languages { get; }
        public SourcesReader Sources { get; }
        public TranslationsReader Translations { get; }
        public UsersReader Users { get; }

        public LocalDbReader(
            ExceptionHandler exceptionHandler,
            EnvironmentService environmentService,
            NetworkService networkService,
            IDataSource realmDataSource)
        {
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _environmentService = environmentService;
            _realmDataSource = realmDataSource;
            _realmDataSource.LocalDatabaseInitialized += LocalDatabase_Initialized;

            Words = new WordsReader();
            Phrases = new PhrasesReader();
            Languages = new LanguagesReader();
            Sources = new SourcesReader();
            Translations = new TranslationsReader();
            Users = new UsersReader();
        }

        private void LocalDatabase_Initialized()
        {
            DataSourceInitialized?.Invoke();
        }
        public void RemoveAllEntries()
        {
            // This method is only used in test, it should never, ever be called in prod

            _realmDataSource?.RemoveAllEntries();
        }

        public void InitializeDataSource()
        {
            _realmDataSource.Initialize();
        }
    }
}
