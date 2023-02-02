using chldr_data.Entities;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.Extensions.Logging;

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
            //var fileService = new FileService(AppContext.BaseDirectory);
            //var exceptionHandler = new ExceptionHandler(new Logger<DataAccessTests>), fileService);
            //var realmService = new RealmService(fileService, );
            //var dataAccess = new DataAccess(fileService, realmService);
            //await dataAccess.Initialize();

            //// ����� ����� ����� - � ������ ������ ��� ������ ����� �� ���� ������
            //var wordToTest = dataAccess.Database.All<Word>().First();

            //// 2. ����
            //var wordById = dataAccess.GetWordById(wordToTest._id);

            //// 3. ��������
            //// �������������� ��� ���������� ��������� ����� ����� ����������� ����� ����������� �� ������ �� ID
            //Assert.Equal(wordToTest.Content, wordById.Content);
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