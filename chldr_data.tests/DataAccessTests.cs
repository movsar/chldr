using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_shared.Dto;

namespace chldr_data.tests
{
    public class DataAccessTests
    {
        private static IDataAccess _dataAccess;

        public DataAccessTests()
        {
            TestSetup();
        }

        private static void TestSetup()
        {
            var dataDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory));

            var fileService = new FileService(dataDirectory.FullName);
            var exceptionHandler = new ExceptionHandler(fileService);
            var networkService = new NetworkService();
            var realmService = new OfflineRealmService(fileService, exceptionHandler);
            var dataAccess = new OfflineDataAccess(new RealmServiceFactory(new List<IRealmService>()
            {
                realmService
            }), exceptionHandler, networkService);
            dataAccess.Initialize();

            _dataAccess = dataAccess;
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
            WordDto newWord = new WordDto()
            {
                Content = "Hello",
                PartOfSpeech = Enums.PartsOfSpeech.Noun,
                GrammaticalClass = 1,
            };

            newWord.Translations.Add(new TranslationDto("TUR")
            {
                Content = "Merhaba",
            });

            var words = _dataAccess.WordsRepository.Add(newWord);
            var word = words.Where(entry => entry is WordModel).First() as WordModel;

            // 2. ����
            var wordById = contentStore.GetWordById(word.Id);

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
            var realmService = new SyncedRealmService(fileService, exceptionHandler);
            var userService = new UserService(networkService, realmService);
            //var dataAccess = new SyncedDataAccess(realmService, exceptionHandler, networkService, userService);
            //await dataAccess.Initialize();

            var badId = new ObjectId("1C1bB21b");

            // 2. ����
            Action callGetWordById = new Action(() =>
            {
                //       var wordById = dataAccess.GetWordById(badId);
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