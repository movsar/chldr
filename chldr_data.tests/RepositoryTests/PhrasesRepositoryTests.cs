using chldr_data.Enums.WordDetails;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Repositories;
using Realms.Sync;

namespace chldr_data.tests.RepositoryTests
{
    internal class PhrasesRepositoryTests : TestsBase
    {
        public async Task GetWordById_ExpectedInput_ReturnsWord()
        {
            // 1. Создаем новый объект Слово со всеми необходимыми зависимостями
            var user= new UserDto();
            WordDto wordToInsert = new WordDto()
            {
                Content = "Hello",
                PartOfSpeech = PartOfSpeech.Noun,
                SourceId = SourcesRepository.GetAllNamedSources().First().SourceId,
            };
            wordToInsert.AdditionalDetails.Classes[0] = 1;
            
            wordToInsert.Translations.Add(new TranslationDto(wordToInsert.EntryId, user.UserId)
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