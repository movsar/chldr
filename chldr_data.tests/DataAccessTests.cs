using chldr_data.Entities;
using chldr_data.Services;

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
            var fileService = new FileService(AppContext.BaseDirectory);
            var realmService = new RealmService();
            var dataAccess = new DataAccess(fileService, realmService);
            await dataAccess.InitializeDatabase();

            // Берем любое слово - в данном случае это первое слово из базы данных
            var wordToTest = dataAccess.Database.All<Word>().First();

            // 2. Тест
            var wordById = dataAccess.GetWordById(wordToTest._id);

            // 3. Проверка
            // Удостоверяемся что содержимое исходного слова равно содержимому слова полученному из метода по ID
            Assert.Equal(wordToTest.Content, wordById.Content);
        }

        public static async Task GetWordById_BadId_ReturnsError()
        {
            // Надо передать неправильный ID и удостовериться что это вызовет ошибку
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