using chldr_data.Entities;
using chldr_data.Models;
using chldr_data.Services;
using chldr_shared.Stores;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace chldr_data.tests
{
    public class DataAccessTests
    {
        /* ���� ������� ������� � ��� ��� �� ���� �� ����� ���������� ������, 
         * ������ ��� � [Theory] � ������� �������� ����� ��������� ����� � ������� �����������
         */
        [Fact]
        /* ��������� ����� ������� ������ ���������� ������ "�����" �� ��� �������� "Id" (��������������)
         * 
         * ������ ������� ���������� {��������������}_{�������������}_RETURNS{������������������}
         * ExpectedInput - ��������� ��������, �.�. ���������� Id ���� ����� ���� Id
         * BadId - ����������� �������������� Id � ��
         */
        public static async Task GetWordById_ExpectedInput_ReturnsWord()
        {
            // 1. ����������

            // �������������� ����������� ������ ����� ��������� ������ �����
            var fileService = new FileService(AppContext.BaseDirectory);
            var exceptionHandler = new ExceptionHandler(fileService);
            var networkService = new NetworkService();
            var realmService = new RealmService(fileService, exceptionHandler);
            var dataAccess = new DataAccess(fileService, realmService, exceptionHandler, networkService);
            await dataAccess.Initialize();

            var contentStore = new ContentStore(dataAccess, exceptionHandler);

            // ����� ����� ����� - � ������ ������ ��� ������ ����� �� ���� ������
            var words = contentStore.GetRandomEntries();
            var word = words.Where(entry => entry is WordModel).First() as WordModel;

            // 2. ����
            var wordById = contentStore.GetWordById(word.WordId);

            // 3. ��������
            // �������������� ��� ���������� ��������� ����� ����� ����������� ����� ����������� �� ������ �� ID
            Assert.Equal(word.Content, wordById.Content);
        }

        [Fact]
        public static async Task GetWordById_BadId_ReturnsError()
        {
            // 1. ����������

            // �������������� ����������� ������ ����� ��������� ������ �����
            var fileService = new FileService(AppContext.BaseDirectory);
            var exceptionHandler = new ExceptionHandler(fileService);
            var networkService = new NetworkService();
            var realmService = new RealmService(fileService, exceptionHandler);
            var dataAccess = new DataAccess(fileService, realmService, exceptionHandler, networkService);
            await dataAccess.Initialize();

            var badId = new ObjectId("1C1bB21b");

            // 2. ����
            Action callGetWordById = new Action(() =>
            {
                var wordById = dataAccess.GetWordById(badId);
            });

            // 3. ��������
            Assert.Throws<System.FormatException>(callGetWordById);
        }

        public static async Task GetWordById_NullId_ReturnsError()
        {
            // ���� �������� null ������ ID � �������������� ��� ��� ������� ������
        }

        public static async Task GetPhraseById_ExpectedInput_ReturnsPhrase()
        {

        }


    }
}