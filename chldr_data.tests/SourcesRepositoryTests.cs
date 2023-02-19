using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Services;
using chldr_utils.Services;
using chldr_utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.tests
{
    public class SourcesRepositoryTests
    {
        private static IDataAccess _dataAccess;

        static SourcesRepositoryTests()
        {
        TestSetup();
        }
        private static void TestSetup()
        {
            var fileService = new FileService(AppContext.BaseDirectory);
            var exceptionHandler = new ExceptionHandler(fileService);
            var networkService = new NetworkService();

            var realmService = new OfflineRealmService(fileService, exceptionHandler);
            var realmServiceFactory = new RealmServiceFactory(new List<IRealmService>() { realmService });

            var dataAccess = new DataAccess(realmServiceFactory, exceptionHandler, networkService,
                new EntriesRepository<EntryModel>(realmServiceFactory),
                new WordsRepository(realmServiceFactory),
                new PhrasesRepository(realmServiceFactory),
                new LanguagesRepository(realmServiceFactory),
                new SourcesRepository(realmServiceFactory));

            dataAccess.RemoveAllEntries();

            _dataAccess = dataAccess;
        }
        [Fact]
        private static void GetUnverifiedSources_NoInput_ReturnsSources()
        {
          var UnverifiedSources = _dataAccess.SourcesRepository.GetUnverifiedSources();


            Assert.True(UnverifiedSources.Count() > 0);
        }

    }

}
    



