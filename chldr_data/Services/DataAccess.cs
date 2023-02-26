using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_utils;
using chldr_utils.Services;

namespace chldr_data.Services
{
    public class DataAccess : IDataAccess
    {
        public event Action<DataSourceType>? DatasourceInitialized;

        protected readonly ExceptionHandler _exceptionHandler;
        protected readonly NetworkService _networkService;
        protected readonly IRealmServiceFactory _realmServiceFactory;
        private readonly EnvironmentService _environmentService;

        public SourcesRepository SourcesRepository { get; }
        public LanguagesRepository LanguagesRepository { get; }
        public WordsRepository WordsRepository { get; }
        public PhrasesRepository PhrasesRepository { get; }
        public EntriesRepository<EntryModel> EntriesRepository { get; }
        public UsersRepository UsersRepository { get; }


        public DataAccess(IRealmServiceFactory realmServiceFactory,
            ExceptionHandler exceptionHandler,
            EnvironmentService environmentService,
            NetworkService networkService,
            EntriesRepository<EntryModel> entriesRepository,
            WordsRepository wordsRepository,
            PhrasesRepository phrasesRepository,
            LanguagesRepository languagesRepository,
            SourcesRepository sourcesRepository,
            UsersRepository usersRepository)
        {
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _realmServiceFactory = realmServiceFactory;
            _environmentService = environmentService;

            EntriesRepository = entriesRepository;
            WordsRepository = wordsRepository;
            PhrasesRepository = phrasesRepository;
            LanguagesRepository = languagesRepository;
            SourcesRepository = sourcesRepository;
            UsersRepository = usersRepository;
        }

        private void DataSource_Initialized(DataSourceType dataSourceType)
        {
            _realmServiceFactory.CurrentDataSource = dataSourceType;
            DatasourceInitialized?.Invoke(dataSourceType);

            // Switch to synced if available
            Task.Run(async () =>
            {
                try
                {
                    if (dataSourceType == DataSourceType.Offline && _networkService.IsNetworUp && _environmentService.CurrentPlatform != chldr_shared.Enums.Platforms.Web)
                    {
                        await Task.Delay(250);
                        ActivateDatasource(DataSourceType.Synced);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("998"))
                    {
                        _exceptionHandler.ProcessDebug(new Exception("NETWORK_ERROR"));
                    }
                    else
                    {
                        _exceptionHandler.ProcessDebug(ex);
                    }
                }
            });
        }

        public void ActivateDatasource(DataSourceType dataSourceType)
        {
            var dataSource = _realmServiceFactory.GetInstance(dataSourceType);
            dataSource.DatasourceInitialized += DataSource_Initialized;
            dataSource.Initialize();
        }

        public void RemoveAllEntries()
        {
            // This method is only used in test, it should never, ever be called in prod
            if (_realmServiceFactory.CurrentDataSource == DataSourceType.Synced)
            {
                return;
            }

            var dataSource = _realmServiceFactory.GetInstance(DataSourceType.Offline) as OfflineRealmService;
            dataSource?.RemoveAllEntries();
        }
    }
}
