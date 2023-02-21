using chldr_data.Dto;
using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;
using chldr_data.tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.tests.RepositoryTests
{
    internal class PhrasesRepositoryTests
    {
        private IDataAccess _dataAccess;

        public PhrasesRepositoryTests()
        {
            _dataAccess = TestDataFactory.CreateDataAccess();
            _dataAccess.RemoveAllEntries();
        }
        public async Task GetWordById_ExpectedInput_ReturnsWord()
        {
            // 1. Создаем новый объект Слово со всеми необходимыми зависимостями
            WordDto wordToInsert = new WordDto()
            {
                Content = "Hello",
                PartOfSpeech = PartOfSpeech.Noun,
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
    }

}
