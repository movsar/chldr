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
using Microsoft.AspNetCore.Identity;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_api.GraphQL.MutationServices;

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
        private static readonly UserFaker _userFaker;
        private static readonly IStringLocalizer<AppLocalizations> _localizer;
        private static readonly IConfigurationRoot _configuration;
        private static readonly EmailService _emailService;
        private static readonly UserManager<SqlUser> _userManager;
        private static readonly SignInManager<SqlUser> _signInManager;

        static TestDataFactory()
        {
            _fileService = new FileService(Path.Combine(AppContext.BaseDirectory, Constants.TestsFileServicePath));
            _exceptionHandler = new ExceptionHandler(_fileService);
            _environmentService = new EnvironmentService(Platforms.Web, true);
            _requestService = new RequestService(new GraphQLClient(_exceptionHandler, _environmentService));

            _entryDtoFaker = new EntryDtoFaker();
            _translationDtoFaker = new TranslationDtoFaker();
            _soundDtoFaker = new SoundDtoFaker();
            _userDtoFaker = new UserDtoFaker();
            _sourceDtoFaker = new SourceDtoFaker();
            _userFaker = new UserFaker();

            var listOfUsers = _userFaker.GenerateBetween(2, 5);

            //var mockExceptionHandler = new Mock<ExceptionHandler>();
            _localizer = GetStringLocalizer();

            _configuration = new ConfigurationBuilder().Build();
            _emailService = CreateFakeEmailService();
            _userManager = CreateFakeUserManager(listOfUsers);
            _signInManager = CreateFakeSignInManager(_userManager);

            Constants.EntriesApproximateCoount = 5000;
        }

        public static IStringLocalizer<AppLocalizations> GetStringLocalizer()
        {
            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            return new StringLocalizer<AppLocalizations>(factory);
        }

        static UserManager<SqlUser> CreateFakeUserManager(List<SqlUser> listOfUsers)
        {
            var store = new Mock<IUserStore<SqlUser>>();
            var userManager = new Mock<UserManager<SqlUser>>(store.Object, null, null, null, null, null, null, null, null);
            userManager.Object.UserValidators.Add(new UserValidator<SqlUser>());
            userManager.Object.PasswordValidators.Add(new PasswordValidator<SqlUser>());

            userManager.Setup(x => x.DeleteAsync(It.IsAny<SqlUser>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.CreateAsync(It.IsAny<SqlUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<SqlUser, string>((x, y) => listOfUsers.Add(x));
            userManager.Setup(x => x.UpdateAsync(It.IsAny<SqlUser>())).ReturnsAsync(IdentityResult.Success);

            return userManager.Object;
        }
        private static Mock<IAuthenticationService> MockAuth(HttpContext context)
        {
            var auth = new Mock<IAuthenticationService>();
            context.RequestServices = new ServiceCollection().AddSingleton(auth.Object).BuildServiceProvider();
            return auth;
        }

        static SignInManager<SqlUser> CreateFakeSignInManager(UserManager<SqlUser> manager)
        {
            // Setup
            var context = new DefaultHttpContext();
            var auth = MockAuth(context);

            // REVIEW: auth changes we lost the ability to mock is persistent
            //var properties = new AuthenticationProperties { IsPersistent = isPersistent };
            var authResult = AuthenticateResult.NoResult();
            auth.Setup(a => a.AuthenticateAsync(context, IdentityConstants.ApplicationScheme))
                .Returns(Task.FromResult(authResult)).Verifiable();

            var signInManager = new Mock<SignInManager<SqlUser>>(manager,
                new HttpContextAccessor { HttpContext = context },
                new Mock<IUserClaimsPrincipalFactory<SqlUser>>().Object,
                null, null, new Mock<IAuthenticationSchemeProvider>().Object, null);

            return signInManager.Object;
        }
        static void AddInitialData(SqlContext context)
        {
            var user = new SqlUser
            {
                Id = "63a816205d1af0e432fba6dd",
                Email = "movsar.dev@gmail.com"
            };

            var source = new SqlSource
            {
                SourceId = "63a816205d1af0e432fba6de",
                UserId = "63a816205d1af0e432fba6dd",
                Name = "User",
                Notes = null,
                CreatedAt = new DateTime(2023, 1, 13, 7, 44, 53),
                UpdatedAt = new DateTime(2023, 1, 13, 7, 44, 53)
            };

            context.Users.Add(user);
            context.Sources.Add(source);
            context.SaveChanges();
        }
        static SqlContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<SqlContext>()
                .UseInMemoryDatabase(databaseName: "whatever")
                .Options;

            var context = new SqlContext(options);

            AddInitialData(context);

            return context;
        }
        public static IDataProvider CreateSqliteDataProvider()
        {


            var dataProvider = new SqlDataProvider(
                    CreateSqliteContext(),
                    _fileService,
                    _exceptionHandler
                );

            return dataProvider;
        }

        private static SqlContext CreateSqliteContext()
        {
            var connectionString = "Data Source=:memory:;";

            // Apply migrations
            var options = new DbContextOptionsBuilder<SqlContext>()
                  .UseSqlite(connectionString)
                  .Options;

            var context = new SqlContext(options);

            AddInitialData(context);

            return context;
        }

        public static IDataProvider CreateMockDataProvider()
        {


            var dataProvider = new SqlDataProvider(
                    CreateInMemoryContext(),
                    _fileService,
                    _exceptionHandler
                );

            return dataProvider;
        }
        public static IDataProvider CreateTestSqlDataProvider()
        {
            // Constants.TestingDatabaseConnectionString
            // Remove sql database
            var dataProvider = new SqlDataProvider(
                null,
                _fileService,
                _exceptionHandler
                );
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
            var dataProvider = CreateTestSqlDataProvider();
            return new EntryService(dataProvider, _requestService, _exceptionHandler);
        }
        public static SourceService CreateSourceService()
        {
            var dataProvider = CreateTestSqlDataProvider();
            return new SourceService(dataProvider, _requestService, _exceptionHandler);
        }

        public static ContentStore CreateContentStore()
        {
            return new ContentStore(_exceptionHandler,
                CreateTestSqlDataProvider(),
                _environmentService,
                CreateSourceService(),
                CreateEntryService(),
                CreateUserService());
        }

        private static UserService CreateUserService()
        {
            var localStorageService = new LocalStorageService(null, _exceptionHandler);
            return new UserService(CreateTestSqlDataProvider(), _requestService, localStorageService);
        }

        public static UserResolver CreateFakeUserResolver(IDataProvider testDataProvider)
        {
            var userResolver = new UserResolver(
                testDataProvider,
                _localizer,
                _emailService,
                _exceptionHandler,
                _fileService,
                _configuration,
                _userManager,
                _signInManager);

            return userResolver;
        }
    }
}
