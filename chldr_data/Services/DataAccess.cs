using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Models.Words;
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

        public DataAccess(ServiceLocator serviceProvider, IRealmServiceFactory realmServiceFactory,
            ExceptionHandler exceptionHandler,
            EnvironmentService environmentService,
            NetworkService networkService)
        {
            _serviceProvider = serviceProvider;
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _realmServiceFactory = realmServiceFactory;
            _environmentService = environmentService;

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
                        SetActiveDataservice(DataSourceType.Synced);
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

        public Repository GetRepository<T>() where T : IPersistentModelBase
        {
            object? repository = null;

            switch (typeof(T).Name)
            {
                case nameof(EntryModel):
                    repository = _serviceProvider.GetService<EntriesRepository<EntryModel>>();
                    break;

                case nameof(WordModel):
                    repository = _serviceProvider.GetService<WordsRepository>();
                    break;

                case nameof(PhraseModel):
                    repository = _serviceProvider.GetService<PhrasesRepository>();
                    break;

                case nameof(LanguageModel):
                    repository = _serviceProvider.GetService<LanguagesRepository>();
                    break;

                case nameof(SourceModel):
                    repository = _serviceProvider.GetService<SourcesRepository>();
                    break;

                case nameof(TranslationModel):
                    repository = _serviceProvider.GetService<TranslationsRepository>();
                    break;

                case nameof(UserModel):
                    repository = _serviceProvider.GetService<UsersRepository>();
                    break;
            }

            if (repository == null)
            {
                throw new ArgumentException("Wrong model name");
            }

            return (Repository)repository;
        }

        public IRealmService GetActiveDataservice()
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
