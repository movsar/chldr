using Android.Content;
using chldr_app.Services;
using chldr_app.Stores;
using chldr_domain.Interfaces;
using chldr_domain.Services;
using chldr_utils.Services;
using core.Interfaces;
using core.Models;
using core.Services;
using Org.BouncyCastle.Asn1.Ocsp;

namespace chldr_android.Services
{
    public class ServiceLocator
    {
        private readonly IDictionary<Type, object> _services;

        public ServiceLocator()
        {
            _services = new Dictionary<Type, object>();
        }

        public void RegisterServices(Context context)
        {
            var fileService = new FileService(Application.Context.FilesDir!.AbsolutePath);
            RegisterService<IFileService>(fileService);

            var exceptionHandler = new ExceptionHandler(GetService<IFileService>());
            RegisterService<IExceptionHandler>(exceptionHandler);

            var environmentService = new EnvironmentService(core.Enums.Platforms.Android, true);
            RegisterService<IEnvironmentService>(environmentService);

            var localStorageService = new JsonFileSettingsService(fileService, exceptionHandler);
            RegisterService<ISettingsService>(localStorageService);

            var graphQl = new GraphQLClient(exceptionHandler, environmentService, localStorageService);
            RegisterService<IGraphQlClient>(graphQl);

            var requestService = new RequestService(graphQl, environmentService);
            RegisterService<IRequestService>(requestService);

            var syncService = new SyncService(requestService, fileService);
            RegisterService<ISyncService>(syncService);

            var dataProvider = new RealmDataProvider(fileService, exceptionHandler, syncService);
            RegisterService<IDataProvider>(dataProvider);

            var entryService = new EntryService(dataProvider, requestService, exceptionHandler, environmentService);
            RegisterService<EntryService>(entryService);

            var sourceService = new SourceService(dataProvider, requestService, exceptionHandler);
            RegisterService<SourceService>(sourceService);

            var settingsService = new AndroidSettingsService(context);
            RegisterService<ISettingsService>(settingsService);

            var userService = new UserService(dataProvider, requestService, settingsService);
            RegisterService<UserService>(userService);

            var entryCacheService = new EntryCacheService();
            RegisterService<EntryCacheService>(entryCacheService);

            var contentStore = new ContentStore(exceptionHandler, dataProvider, environmentService, sourceService, entryService, userService, entryCacheService);
            RegisterService<ContentStore>(contentStore);
        }

        public void RegisterService<TService>(TService implementation) where TService : class
        {
            _services[typeof(TService)] = implementation ?? throw new ArgumentNullException(nameof(implementation));
        }

        public TService GetService<TService>() where TService : class
        {
            if (_services.TryGetValue(typeof(TService), out var service))
            {
                return (TService)service;
            }

            throw new InvalidOperationException($"Service not registered: {typeof(TService)}");
        }
    }

}
