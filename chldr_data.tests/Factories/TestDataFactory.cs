using chldr_data.Dto;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Services;

namespace chldr_data.tests.Services
{
    public static class TestDataFactory
    {
        private static readonly FileService _fileService;
        private static readonly ExceptionHandler _exceptionHandler;
        private static readonly NetworkService _networkService;
        private static readonly DataAccess _dataAccess;

        static TestDataFactory()
        {
            _fileService = new FileService(AppContext.BaseDirectory);
            _exceptionHandler = new ExceptionHandler(_fileService);
            _networkService = new NetworkService();

            var realmService = new OfflineRealmService(_fileService, _exceptionHandler);
            var realmServiceFactory = new RealmServiceFactory(new List<IRealmService>() { realmService });

            _dataAccess = new DataAccess(realmServiceFactory, _exceptionHandler, _networkService,
                new EntriesRepository<EntryModel>(realmServiceFactory),
                new WordsRepository(realmServiceFactory),
                new PhrasesRepository(realmServiceFactory),
                new LanguagesRepository(realmServiceFactory),
                new SourcesRepository(realmServiceFactory),
                new UsersRepository(realmServiceFactory));
                
            _dataAccess.ActivateDatasource(Enums.DataSourceType.Offline);    
        }

        public static IDataAccess GetTestDataAccess()
        {
            return _dataAccess;
        }

        internal static WordDto CreateWordDto(string content, string notes, string languageCode, string translation)
        {
            var source = _dataAccess.SourcesRepository.GetAllNamedSources()[0];
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
