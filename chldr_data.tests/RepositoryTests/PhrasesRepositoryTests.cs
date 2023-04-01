using chldr_data.Dto;
using chldr_data.Enums.WordDetails;
using chldr_data.Repositories;

namespace chldr_data.tests.RepositoryTests
{
    internal class PhrasesRepositoryTests : TestsBase
    {
        public async Task GetWordById_ExpectedInput_ReturnsWord()
        {
            // 1. Создаем новый объект Слово со всеми необходимыми зависимостями
            WordDto wordToInsert = new WordDto()
            {
                Content = "Hello",
                PartOfSpeech = PartOfSpeech.Noun,
                SourceId = SourcesRepository.GetAllNamedSources().First().Id.ToString(),
            };
            wordToInsert.Classes[0] = 1;

            wordToInsert.Translations.Add(new TranslationDto("RUS")
            {
                Content = "Привет",
            });

            // Вставляем в базу данных и получаем уникальный идентификатор вставленного объекта "Word"
            var insertedWordId = WordsRepository.Insert(wordToInsert);

            // 2. Тестируем метод GetById - пытаемся получить из базы данных добавленное слово
            var insertedWord = WordsRepository.GetById(insertedWordId);

            // 3. Проверяем
            Assert.Equal(wordToInsert.Content, insertedWord.Content);
        }
    }

}
