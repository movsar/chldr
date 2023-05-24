using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Repositories;
using chldr_utils;
using chldr_utils.Services;
using Realms;

namespace chldr_data.Services
{
    public class DataAccess : IDataAccess
    {
        public event Action? DataAccessInitialized;

        private readonly ServiceLocator _serviceProvider;
        protected readonly ExceptionHandler _exceptionHandler;
        protected readonly NetworkService _networkService;
        private readonly EnvironmentService _environmentService;
        private readonly IDataSourceService _realmDataSource;

        public IGraphQLRequestSender RequestSender { get; set; }

        public DataAccess(
            ServiceLocator serviceProvider,
            ExceptionHandler exceptionHandler,
            EnvironmentService environmentService,
            NetworkService networkService,
            IDataSourceService realmDataSource,
            IGraphQLRequestSender requestSender
            )
        {
            _serviceProvider = serviceProvider;
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _environmentService = environmentService;

            _realmDataSource = realmDataSource;
            _realmDataSource.LocalDatabaseInitialized += LocalDatabase_Initialized;

            RequestSender = requestSender;

            serviceProvider.Register(new EntriesRepository<EntryModel>(this));
            serviceProvider.Register(new WordsRepository(this));
            serviceProvider.Register(new PhrasesRepository(this));
            serviceProvider.Register(new LanguagesRepository(this));
            serviceProvider.Register(new SourcesRepository(this));
            serviceProvider.Register(new TranslationsRepository(this));
            serviceProvider.Register(new UsersRepository(this));
        }

        private void LocalDatabase_Initialized()
        {
            DataAccessInitialized?.Invoke();
        }

        public Repository GetRepository<T>() where T : IEntity
        {
            object? repository = null;

            switch (typeof(T).Name)
            {
                case nameof(IEntryEntity):
                    repository = _serviceProvider.GetService<EntriesRepository<EntryModel>>();
                    break;

                case nameof(IWordEntity):
                    repository = _serviceProvider.GetService<WordsRepository>();
                    break;

                case nameof(IPhraseEntity):
                    repository = _serviceProvider.GetService<PhrasesRepository>();
                    break;

                case nameof(ILanguageEntity):
                    repository = _serviceProvider.GetService<LanguagesRepository>();
                    break;

                case nameof(ISourceEntity):
                    repository = _serviceProvider.GetService<SourcesRepository>();
                    break;

                case nameof(ITranslationEntity):
                    repository = _serviceProvider.GetService<TranslationsRepository>();
                    break;

                case nameof(IUserEntity):
                    repository = _serviceProvider.GetService<UsersRepository>();
                    break;
            }

            if (repository == null)
            {
                throw new ArgumentException("Wrong model name");
            }

            return (Repository)repository;
        }

        public void RemoveAllEntries()
        {
            // This method is only used in test, it should never, ever be called in prod

            _realmDataSource?.RemoveAllEntries();
        }

        public Realm GetDatabase()
        {
            return _realmDataSource.GetDatabase();
        }

        public void InitializeDataSource()
        {
            _realmDataSource.InitializeDatabase();
        }
    }
}
