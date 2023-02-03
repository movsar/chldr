using chldr_data.Factories;
using chldr_data.Interfaces;

namespace chldr_data.tests
{
    public class DataAccessTests
    {
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
            // 1. Подготовка

            // Инициализируем необходимые классы чтобы запустить нужный метод
            var dataDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory));

            var fileService = new FileService(dataDirectory.FullName);
            var exceptionHandler = new ExceptionHandler(fileService);
            var networkService = new NetworkService();
            var realmService = new OfflineRealmService(fileService, exceptionHandler);
            var dataAccess = new OfflineDataAccess(new RealmServiceFactory(new List<IRealmService>()
            {
                realmService
            }), exceptionHandler, networkService);
            dataAccess.Initialize();

            // var contentStore = new ContentStore(new DataAccessFactory(new List<IDataAccess>() { dataAccess }), exceptionHandler);

            // Берем любое слово - в данном случае это первое слово из базы данных
            

            var words = dataAccess.WordsRepository.Add(word);
            var word = words.Where(entry => entry is WordModel).First() as WordModel;

            // 2. Тест
            var wordById = contentStore.GetWordById(word.Id);

            // 3. Проверка
            // Удостоверяемся что содержимое исходного слова равно содержимому слова полученному из метода по ID
            Assert.Equal(word.Content, wordById.Content);
        }

        [Fact]
        public static async Task GetWordById_BadId_ReturnsError()
        {
            // 1. Подготовка

            // Инициализируем необходимые классы чтобы запустить нужный метод
            var fileService = new FileService(AppContext.BaseDirectory);
            var exceptionHandler = new ExceptionHandler(fileService);
            var networkService = new NetworkService();
            var realmService = new SyncedRealmService(fileService, exceptionHandler);
            var userService = new UserService(networkService, realmService);
            //var dataAccess = new SyncedDataAccess(realmService, exceptionHandler, networkService, userService);
            //await dataAccess.Initialize();

            var badId = new ObjectId("1C1bB21b");

            // 2. Тест
            Action callGetWordById = new Action(() =>
            {
                //       var wordById = dataAccess.GetWordById(badId);
            });

            // 3. Проверка
            Assert.Throws<System.FormatException>(callGetWordById);
        }

        public static async Task GetWordById_NullId_ReturnsError()
        {
            // Надо передать null вместо ID и удостовериться что это вызовет ошибку
        }

        public static async Task GetPhraseById_ExpectedInput_ReturnsPhrase()
        {

        }


    }
}