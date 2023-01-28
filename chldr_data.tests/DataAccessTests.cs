using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Services;

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
            var realmService = new RealmService();
            var dataAccess = new DataAccess(fileService, realmService);
            await dataAccess.InitializeDatabase();

            // ����� ����� ����� - � ������ ������ ��� ������ ����� �� ���� ������
            var wordToTest = dataAccess.Database.All<Word>().First();

            // 2. ����
            var wordById = dataAccess.GetWordById(wordToTest._id);

            // 3. ��������
            // �������������� ��� ���������� ��������� ����� ����� ����������� ����� ����������� �� ������ �� ID
            Assert.Equal(wordToTest.Content, wordById.Content);
        }

        [Fact]
        public static async Task GetWordByIdBadIdReturnsError()
        {
            var fileService = new FileService(AppContext.BaseDirectory);
            var realmService = new RealmService();
            var dataAccess = new DataAccess(fileService, realmService);
            await dataAccess.InitializeDatabase();

            var wordToTest = dataAccess.Database.All<Word>().First();

            var wordById = dataAccess.GetWordById(wordToTest._id);

            Assert.Equal(wordById.Content, wordToTest.Content);


            // ���� �������� ������������ ID � �������������� ��� ��� ������� ������
        }

        [Fact]
        public static async Task GetPhraseById_NullId_ReturnsError()
        {
            Object p = null;
            // ���� �������� null ������ ID � �������������� ��� ��� ������� ������
            var fileService = new FileService(AppContext.BaseDirectory);
            var realmService = new RealmService();
            var dataAccess = new DataAccess(fileService, realmService);
            await dataAccess.InitializeDatabase();

            var wordToTest = dataAccess.Database.All<Phrase>().First().Content;

            var wordById = dataAccess.GetPhraseById;

            Assert.Null(wordById);
        }

        [Fact]
        public static async Task GetPhraseById_ExpectedInput_ReturnsPhrase()
        {
            var fileService = new FileService(AppContext.BaseDirectory);
            var realmService = new RealmService();
            var dataAccess = new DataAccess(fileService, realmService);
            await dataAccess.InitializeDatabase();

            var wordToTest = dataAccess.Database.All <Phrase>().First().Content;

            var wordById = dataAccess.GetPhraseById;

            Assert.True(wordById.Equals(wordToTest));

        }







}
}