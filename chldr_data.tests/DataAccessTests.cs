using chldr_data.Dto;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Services;

namespace chldr_data.tests
{
    public class DataAccessTests
    {
        private static IDataAccess _dataAccess;

        static DataAccessTests()
        {
            TestSetup();
        }

        private static void TestSetup()
        {
            var fileService = new FileService(AppContext.BaseDirectory);
            var exceptionHandler = new ExceptionHandler(fileService);
            var networkService = new NetworkService();
            var realmService = new OfflineRealmService(fileService, exceptionHandler);
            var dataAccess = new OfflineDataAccess(new RealmServiceFactory(new List<IRealmService>()
            {
                realmService
            }), exceptionHandler, networkService);
            dataAccess.Initialize();

            _dataAccess = dataAccess;
            _dataAccess.Database.Write(() =>
            {
                _dataAccess.Database.RemoveAll<Entry>();
                _dataAccess.Database.RemoveAll<Text>();
                _dataAccess.Database.RemoveAll<Word>();
                _dataAccess.Database.RemoveAll<Phrase>();
                _dataAccess.Database.RemoveAll<Translation>();
            });
            //_contentStore = new ContentStore(new DataAccessFactory(new List<IDataAccess>() { dataAccess }), exceptionHandler);
        }

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
            // 1. ������� ����� ������ ����� �� ����� ������������ �������������
            WordDto wordToInsert = new WordDto()
            {
                Content = "Hello",
                PartOfSpeech = Enums.PartsOfSpeech.Noun,
                GrammaticalClass = 1,
                SourceId = _dataAccess.Database.All<Source>().First()._id,
            };

            wordToInsert.Translations.Add(new TranslationDto("RUS")
            {
                Content = "������",
            });

            // ��������� � ���� ������ � �������� ���������� ������������� ������������ ������� "Word"
            var insertedWordId = _dataAccess.WordsRepository.Insert(wordToInsert);

            // 2. ��������� ����� GetById - �������� �������� �� ���� ������ ����������� �����
            var insertedWord = _dataAccess.WordsRepository.GetById(insertedWordId);

            // 3. ���������
            Assert.Equal(wordToInsert.Content, insertedWord.Content);
        }

        [Fact]
        public static async Task GetWordById_BadId_ReturnsError()
        {
            // 1. �������������� �������� ������������ id
            ObjectId badId = new ObjectId(12, 123, 321, 12);

            // 2. ����
            var wordById = _dataAccess.WordsRepository.GetById(badId);

            // 3. ��������
            // TODO: ������� �������, �������� ��������� ������, ������ ���� 2 � try - catch � ������ � catch ��������� �� ������
            // � ��� ��� ����������
        }

    }
}