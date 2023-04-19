using chldr_data.Repositories;
using chldr_data.tests.Services;

namespace chldr_data.tests.RepositoryTests
{
    public class WordsRepositoryTests : TestsBase
    {
        [Fact]
        public async void Insert_ExpectedInput_ReturnsId()
        {
            // Create a test word dto
            var testWord = TestDataFactory.CreateWordDto("Something", "Whatever", "RUS", "Нечто");

            // Set an existing source
            var sources = SourcesRepository.GetAllNamedSources();
            testWord.SourceId = sources[0].Id.ToString();

            // Insert
            var insertedWordId = WordsRepository.Insert(testWord);

            Assert.NotEqual(string.Empty, insertedWordId);
        }

        [Fact]
        public async Task GetWordById_BadId_ReturnsError()
        {
            // 1. Подготавливаем заведомо неправильный id
            string badId ="2, 123, 321, 12";


            // 2. Тест
            {
                try
                {
                    var wordById = WordsRepository.GetById(badId);
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
        public void GetRandomWords_ExpectedInput_ReturnsListOfWord()
        {

            var randomWord = TestDataFactory.CreateWordDto("Time", "When", "RUS", "Когда");
            var someWord = TestDataFactory.CreateWordDto("Object", "Car", "RUS", "Машина");

            var addrandomWord = WordsRepository.Insert(randomWord);
            var addsomeWord = WordsRepository.Insert(someWord);
            var words = WordsRepository.GetRandomWords(2);
            Assert.True(words.Count() == 2);
        }
    }
}
