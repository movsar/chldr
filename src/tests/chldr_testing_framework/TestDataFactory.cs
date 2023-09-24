using chldr_utils.Services;
using chldr_utils;
using Microsoft.EntityFrameworkCore;
using chldr_data.Interfaces;
using chldr_data;
using chldr_data.Services;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.remote.Services;
using chldr_data.local.Services;
using chldr_shared.Enums;
using chldr_test_utils.Generators;
using chldr_data.Resources.Localizations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Google.Protobuf.WellKnownTypes;
using chldr_data.remote.SqlEntities;
using MimeKit;
using MailKit.Net.Smtp;
using chldr_testing_framework.Generators;
using chldr_utils.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using chldr_shared.Services;
using chldr_shared.Stores;

namespace chldr_test_utils
{
    public static class TestDataFactory
    {
        // ! We don't use InMemory databases because they don't support transactions used in almost every CRUD operation

        private static readonly FileService _fileService;
        private static readonly ExceptionHandler _exceptionHandler;
        private static readonly EnvironmentService _environmentService;
        private static readonly RequestService _requestService;
        private static readonly EntryDtoFaker _entryDtoFaker;
        private static readonly TranslationDtoFaker _translationDtoFaker;
        private static readonly SoundDtoFaker _soundDtoFaker;
        private static readonly UserDtoFaker _userDtoFaker;
        private static readonly SourceDtoFaker _sourceDtoFaker;

        static TestDataFactory()
        {
            _fileService = new FileService(Path.Combine(AppContext.BaseDirectory, Constants.TestsFileServicePath));
            _exceptionHandler = new ExceptionHandler(_fileService);
            _environmentService = new EnvironmentService(Platforms.Windows, true);
            _requestService = new RequestService(new GraphQLClient(_exceptionHandler, _environmentService));

            _entryDtoFaker = new EntryDtoFaker();
            _translationDtoFaker = new TranslationDtoFaker();
            _soundDtoFaker = new SoundDtoFaker();
            _userDtoFaker = new UserDtoFaker();
            _sourceDtoFaker = new SourceDtoFaker();

            Constants.EntriesApproximateCoount = 5000;
        }

        public static IStringLocalizer<AppLocalizations> GetStringLocalizer()
        {
            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            return new StringLocalizer<AppLocalizations>(factory);
        }
        public static IDataProvider CreateSqlDataProvider()
        {
            // Remove sql database
            var dataProvider = new SqlDataProvider(_fileService, _exceptionHandler, Constants.TestingDatabaseConnectionString);
            return dataProvider;
        }
        public static IDataProvider CreatRealmDataProvider()
        {
            if (File.Exists(_fileService.OfflineDatabaseFilePath))
            {
                File.Delete(_fileService.OfflineDatabaseFilePath);
            }

            var syncService = new SyncService(_requestService, _fileService);
            var dataProvider = new RealmDataProvider(_fileService, _exceptionHandler, _requestService, syncService);
            return dataProvider;
        }
        public static EmailService CreateFakeEmailService()
        {
            // Create a mock of ISmtpClientWrapper
            var smtpClientMock = new Mock<ISmtpClientWrapper>();

            // Configure the Connect method
            smtpClientMock.Setup(c => c.Connect("mail.hosting.reg.ru", 465, true));

            // Configure the Authenticate method
            smtpClientMock.Setup(c => c.Authenticate("support@dosham.app", "6MsyThgtYWiFTND"));

            // Configure the Send method to do nothing (successful)
            smtpClientMock.Setup(c => c.Send(It.IsAny<MimeMessage>()));

            // Create an instance of EmailService with the mocked ISmtpClientWrapper
            var emailService = new EmailService(() => smtpClientMock.Object)
            {
                Username = "movsar@gmail.com",
                Password = "whatever",
            };

            return emailService;
        }
        public static UserDto CreateRandomUserDto()
        {
            return _userDtoFaker.Generate();
        }

        public static EntryDto CreateRandomEntryDto(string userId, string sourceId)
        {
            var entryDto = _entryDtoFaker.Generate();
            entryDto.UserId = userId;
            entryDto.SourceId = sourceId;

            var translationDto = _translationDtoFaker.Generate();
            translationDto.EntryId = entryDto.EntryId;
            translationDto.UserId = userId;

            var soundDto = _soundDtoFaker.Generate();
            soundDto.EntryId = entryDto.EntryId;
            soundDto.UserId = userId;

            entryDto.TranslationsDtos.Add(translationDto);
            entryDto.SoundDtos.Add(soundDto);

            return entryDto;
        }

        public static SourceDto CreateRandomSourceDto()
        {
            return _sourceDtoFaker.Generate();
        }

        public static TranslationDto CreateRandomTranslationDto(string entryId, string userId)
        {
            var translationDto = _translationDtoFaker.Generate();
            translationDto.UserId = userId;
            translationDto.EntryId = entryId;
            return translationDto;
        }

        public static EntryService CreateEntryService()
        {
            var dataProvider = CreateSqlDataProvider();
            return new EntryService(dataProvider, _requestService, _exceptionHandler);
        }
        public static SourceService CreateSourceService()
        {
            var dataProvider = CreateSqlDataProvider();
            return new SourceService(dataProvider, _requestService, _exceptionHandler);
        }

        public static ContentStore CreateContentStore()
        {
            return new ContentStore(_exceptionHandler,
                CreateSqlDataProvider(),
                _environmentService,
                CreateSourceService(),
                CreateEntryService(),
                CreateUserService());
        }

        private static UserService CreateUserService()
        {
            var localStorageService = new LocalStorageService(null, _exceptionHandler);
            return new UserService(CreateSqlDataProvider(), _requestService, localStorageService);
        }
    }
}
