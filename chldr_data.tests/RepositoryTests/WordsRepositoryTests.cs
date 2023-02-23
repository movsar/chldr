using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Models.Words;
using chldr_data.tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.tests.RepositoryTests
{
    public class WordsRepositoryTests
    {
        private IDataAccess _dataAccess;

        public WordsRepositoryTests()
        {
            _dataAccess = TestDataFactory.GetTestDataAccess();
            _dataAccess.RemoveAllEntries();
        }

        [Fact]
        public async void Insert_ExpectedInput_ReturnsId()
        {
            var testWord = TestDataFactory.CreateWordDto("Something", "Whatever", "RUS", "Нечто");
            
            var insertedWordId = _dataAccess.WordsRepository.Insert(testWord);
            
            Assert.NotEqual(ObjectId.Empty, insertedWordId);
        }

        [Fact]
        public async Task GetWordById_BadId_ReturnsError()
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
        public void GetRandomWords_ExpectedInput_ReturnsListOfWord()
        {
           
            var randomWord = TestDataFactory.CreateWordDto("Time", "When", "RUS", "Когда");
            var someWord = TestDataFactory.CreateWordDto("Object", "Car", "RUS", "Машина");
            
           
            var words = _dataAccess.WordsRepository.GetRandomWords(2);
            Assert.True(words.Count() == 2);
        }
    }
}
