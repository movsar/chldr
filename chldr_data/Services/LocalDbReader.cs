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
        public WordQueries Words { get; }
        public PhraseQueries Phrases { get; }
        public LanguageQueries Languages { get; }
        public SourceQueries Sources { get; }
        public TranslationQueries Translations { get; }
        public UserQueries Users { get; }

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

            Words = new WordQueries();
            Phrases = new PhraseQueries();
            Languages = new LanguageQueries();
            Sources = new SourceQueries();
            Translations = new TranslationQueries();
            Users = new UserQueries();
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
