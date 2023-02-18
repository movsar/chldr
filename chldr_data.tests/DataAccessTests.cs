using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using System.Linq;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using static Realms.Sync.MongoClient;

namespace chldr_data.tests
{
    public class DataAccessTests
    {
        private static IDataAccess _dataAccess;

        static DataAccessTests()
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
            //_contentStore = new ContentStore(new DataAccessFactory(new List<IDataAccess>() { dataAccess }), exceptionHandler);
        }

        /* Этот атрибут говорит о том что на вход не будет подаваться ничего, 
         * бывает еще и [Theory] с помощью которого можно запускать метод с разными параметрами
         */
        [Fact]
        /* Тестируем метод который должен возвращать объект "слово" по его свойству "Id" (идентификатору)
         * 
         * Следуй шаблону именования {НАЗВАНИЕМЕТОДА}_{ТИППАРАМЕТРОВ}_RETURNS{ОЖИДАЕМЫЙРЕЗУЛЬТАТ}
         * ExpectedInput - ожидаемые параметр, т.е. правильный Id если метод ждет Id
         * BadId - неправильно сформированный Id и тд
         */
        public static async Task GetWordById_ExpectedInput_ReturnsWord()
        {
            // 1. Создаем новый объект Слово со всеми необходимыми зависимостями
            WordDto wordToInsert = new WordDto()
            {
                Content = "Hello",
                PartOfSpeech = Enums.PartsOfSpeech.Noun,
                SourceId = _dataAccess.SourcesRepository.GetAllNamedSources().First().Id.ToString(),
            };
            wordToInsert.GrammaticalClasses.Add(1);

            wordToInsert.Translations.Add(new TranslationDto("RUS")
            {
                Content = "Привет",
            });

            // Вставляем в базу данных и получаем уникальный идентификатор вставленного объекта "Word"
            var insertedWordId = _dataAccess.WordsRepository.Insert(wordToInsert);

            // 2. Тестируем метод GetById - пытаемся получить из базы данных добавленное слово
            var insertedWord = _dataAccess.WordsRepository.GetById(insertedWordId);

            // 3. Проверяем
            Assert.Equal(wordToInsert.Content, insertedWord.Content);
        }

        [Fact]
        public static async Task GetWordById_BadId_ReturnsError()
        {
            // 1. Подготавливаем заведомо неправильный id
            ObjectId badId = new ObjectId(12, 123, 321, 12);


            // 2. Тест
            {
                try
                {
                    var wordById = _dataAccess.WordsRepository.GetById(badId);
                }
                catch (System.Exception oshibka)
                {
                    // 3. Проверка
                    // TODO: Сначала запусти, скопируй сообщение ошибки, оберни этап 2 в try - catch и сравни в catch сообщение из ошибки
                    // с тем что скопировал
                    Assert.Equal("There is no such word in the database", oshibka.Message);
                }

            }

        }
        [Fact]
        public void GetAllLanguages_NoInput_ReturnsListOfLanguages()
        {
        var allLanguages = _dataAccess.LanguagesRepository.GetAllLanguages();    


            Assert.True(allLanguages.Count() > 0);           
        }

        }
    }







