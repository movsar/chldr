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
        private readonly IDataSourceService _realmDataSource;

        public IGraphQLRequestSender RequestSender { get; set; }
        public WordReader Words { get; }
        public PhraseReader Phrases { get; }
        public LanguageReader Languages { get; }
        public SourceReader Sources { get; }
        public TranslationReader Translations { get; }
        public UserReader Users { get; }

        public LocalDbReader(
            ExceptionHandler exceptionHandler,
            EnvironmentService environmentService,
            NetworkService networkService,
            IDataSourceService realmDataSource)
        {
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _environmentService = environmentService;
            _realmDataSource = realmDataSource;
            _realmDataSource.LocalDatabaseInitialized += LocalDatabase_Initialized;

            Words = new WordReader();
            Phrases = new PhraseReader();
            Languages = new LanguageReader();
            Sources = new SourceReader();
            Translations = new TranslationReader();
            Users = new UserReader();
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
            _realmDataSource.InitializeDatabase();
        }
    }
}
