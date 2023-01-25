using chldr_data.Entities;
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

        public static async Task GetWordById_BadId_ReturnsError()
        {
            // ���� �������� ������������ ID � �������������� ��� ��� ������� ������
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