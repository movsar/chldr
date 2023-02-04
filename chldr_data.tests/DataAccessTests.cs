using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Factories;
using chldr_data.Interfaces;

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
            var dataAccess = new OfflineDataAccess(new RealmServiceFactory(new List<IRealmService>()
            {
                realmService
            }), exceptionHandler, networkService);
            dataAccess.Initialize();

            _dataAccess = dataAccess;
            _dataAccess.Database.Write(() =>
            {
                _dataAccess.Database.RemoveAll<Entry>();
                _dataAccess.Database.RemoveAll<Text>();
                _dataAccess.Database.RemoveAll<Word>();
                _dataAccess.Database.RemoveAll<Phrase>();
                _dataAccess.Database.RemoveAll<Translation>();
            });
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
                GrammaticalClass = 1,
                SourceId = _dataAccess.Database.All<Source>().First()._id,
            };

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

    }
}