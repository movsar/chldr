using chldr_data.Enums;
using chldr_data.Interfaces;

using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Repositories;
using chldr_utils;
using chldr_utils.Services;
using System;
using System.Collections.Specialized;

namespace chldr_data.Services
{
    public class DataAccess : IDataAccess
    {
        public event Action<DataSourceType>? DatasourceInitialized;

        private readonly ServiceLocator _serviceProvider;
        protected readonly ExceptionHandler _exceptionHandler;
        protected readonly NetworkService _networkService;
        protected readonly IRealmServiceFactory _realmServiceFactory;
        private readonly EnvironmentService _environmentService;
        public IGraphQLRequestSender RequestSender { get; set; }

        public DataAccess(
            ServiceLocator serviceProvider,
            IRealmServiceFactory realmServiceFactory,
            ExceptionHandler exceptionHandler,
            EnvironmentService environmentService,
            NetworkService networkService,
            IGraphQLRequestSender requestSender
            )
        {
            _serviceProvider = serviceProvider;
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _realmServiceFactory = realmServiceFactory;
            _environmentService = environmentService;

            RequestSender = requestSender;

            serviceProvider.Register(new EntriesRepository<EntryModel>(this));
            serviceProvider.Register(new WordsRepository(this));
            serviceProvider.Register(new PhrasesRepository(this));
            serviceProvider.Register(new LanguagesRepository(this));
            serviceProvider.Register(new SourcesRepository(this));
            serviceProvider.Register(new TranslationsRepository(this));
            serviceProvider.Register(new UsersRepository(this));
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
                        //SetActiveDataservice(DataSourceType.Synced);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("998"))
                    {
                        _exceptionHandler.LogError(new Exception("NETWORK_ERROR"));
                    }
                    else
                    {
                        _exceptionHandler.LogError(ex);
                    }
                }
            });
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

        public IDataSourceService GetActiveDataservice()
        {
            var dataSource = _realmServiceFactory.GetActiveInstance();
            return dataSource;
        }

        public void SetActiveDataservice(DataSourceType dataSourceType)
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
