using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Repositories;
using GraphQL.Validation;

namespace chldr_data.tests.Services
{
    internal static class TestDataFactory
    {
        private static readonly FileService _fileService;
        private static readonly ExceptionHandler _exceptionHandler;
        private static readonly NetworkService _networkService;
        private static readonly EnvironmentService _environmentService;
        private static readonly LocalDbReader _dataAccess;
        private static SourcesRepository SourcesRepository => (SourcesRepository)_dataAccess.GetRepository<SourceModel>();

        static TestDataFactory()
        {
            _fileService = new FileService(AppContext.BaseDirectory);
            _exceptionHandler = new ExceptionHandler(_fileService);
            _networkService = new NetworkService();
            _environmentService = new EnvironmentService(chldr_shared.Enums.Platforms.Windows);

            var realmService = new RealmDataSource(_fileService, _exceptionHandler);
            var serviceLocator = new ServiceLocator();
            var requestSender = new GraphQLRequestSender(_exceptionHandler);
            var realmDataSource = new RealmDataSource(_fileService, _exceptionHandler);
            _dataAccess = new DataAccess(serviceLocator, _exceptionHandler, _environmentService, _networkService, realmDataSource, requestSender);
        }

        internal static ILocalDbReader GetTestDataAccess()
        {
            return _dataAccess;
        }

        internal static WordDto CreateWordDto(string content, string notes, string languageCode, string translation)
        {

            var source = SourcesRepository.GetAllNamedSources()[0];
            var wordDto = new WordDto();
            wordDto.Content = content;
            wordDto.Notes = notes;
            wordDto.SourceId = source.SourceId;
            wordDto.Translations.Add(new TranslationDto()
            {
                Content = translation,
                Rate = 5
            });
            wordDto.PartOfSpeech = Enums.WordDetails.PartOfSpeech.Noun;
            return wordDto;
        }
    }
}
