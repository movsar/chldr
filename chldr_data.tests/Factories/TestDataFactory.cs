using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;

namespace chldr_data.tests.Services
{
    public static class TestDataFactory
    {
        private static readonly FileService _fileService;
        private static readonly ExceptionHandler _exceptionHandler;
        private static readonly NetworkService _networkService;

        static TestDataFactory()
        {
            _fileService = new FileService(AppContext.BaseDirectory);
            _exceptionHandler = new ExceptionHandler(_fileService);
            _networkService = new NetworkService();
        }

        public static IDataAccess CreateDataAccess()
        {
            var realmService = new OfflineRealmService(_fileService, _exceptionHandler);
            var realmServiceFactory = new RealmServiceFactory(new List<IRealmService>() { realmService });

            var dataAccess = new DataAccess(realmServiceFactory, _exceptionHandler, _networkService,
                new EntriesRepository<EntryModel>(realmServiceFactory),
                new WordsRepository(realmServiceFactory),
                new PhrasesRepository(realmServiceFactory),
                new LanguagesRepository(realmServiceFactory),
                new SourcesRepository(realmServiceFactory));

            return dataAccess;
        }
    }
}
