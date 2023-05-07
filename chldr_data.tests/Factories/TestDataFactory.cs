using chldr_data.Dto;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;

namespace chldr_data.tests.Services
{
    internal static class TestDataFactory
    {
        private static readonly FileService _fileService;
        private static readonly ExceptionHandler _exceptionHandler;
        private static readonly NetworkService _networkService;
        private static readonly EnvironmentService _environmentService;
        private static readonly DataAccess _dataAccess;
        private static SourcesRepository SourcesRepository => (SourcesRepository)_dataAccess.GetRepository<SourceModel>();

        static TestDataFactory()
        {
            _fileService = new FileService(AppContext.BaseDirectory);
            _exceptionHandler = new ExceptionHandler(_fileService);
            _networkService = new NetworkService();
            _environmentService = new EnvironmentService(chldr_shared.Enums.Platforms.Windows);

            var realmService = new OfflineRealmService(_fileService, _exceptionHandler);
            var realmServiceFactory = new RealmServiceFactory(new List<IDataSourceService>() { realmService });
            var serviceLocator = new ServiceLocator();

            _dataAccess = new DataAccess(serviceLocator, realmServiceFactory, _exceptionHandler, _environmentService, _networkService);
            _dataAccess.SetActiveDataservice(Enums.DataSourceType.Offline);
        }

        internal static IDataAccess GetTestDataAccess()
        {
            return _dataAccess;
        }

        internal static WordDto CreateWordDto(string content, string notes, string languageCode, string translation)
        {

            var source = SourcesRepository.GetAllNamedSources()[0];
            var wordDto = new WordDto();
            wordDto.Content = content;
            wordDto.Notes = notes;
            wordDto.SourceId = source.Id.ToString();
            wordDto.Translations.Add(new TranslationDto(languageCode)
            {
                Content = translation,
                Rate = 5
            });
            wordDto.PartOfSpeech = Enums.WordDetails.PartOfSpeech.Noun;
            return wordDto;
        }
    }
}
