using chldr_api.GraphQL.MutationServices;
using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Resources.Localizations;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace chldr_api.tests.ServiceResolverTests
{
    public class RegisterUserResolverTests
    {
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly EmailService _emailService;

        public RegisterUserResolverTests()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
          //  _localizer = new Mock<IStringLocalizer<AppLocalizations>>().Object;
            _emailService = new Mock<EmailService>().Object;

            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            _localizer = new StringLocalizer<AppLocalizations>(factory);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInput_ReturnsSuccessResponse()
        {
            // Arrange
            using var dbContext = new SqlContext(CreateNewContextOptions());
            var email = "test@example.com";
            var password = "password";
            var firstName = "John";
            var lastName = "Doe";
            var patronymic = "Smith";

            var registerUserResolver = new RegisterUserResolver();

            // Act
            var response = await registerUserResolver.ExecuteAsync(
                dbContext, _localizer, _emailService,
                email, password, firstName, lastName, patronymic);

            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response.Token);
        }

        [Fact]
        public async Task ExecuteAsync_WithExistingEmail_ReturnsErrorResponse()
        {
            // Arrange
            using var dbContext = new SqlContext(CreateNewContextOptions());
            var email = "existing@example.com";
            var password = "password";
            var firstName = "John";
            var lastName = "Doe";
            var patronymic = "Smith";

            // Add existing user to the database
            dbContext.Users.Add(new SqlUser
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                UserStatus = (int)UserStatus.Unconfirmed,
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic
            });
            await dbContext.SaveChangesAsync();

            var registerUserResolver = new RegisterUserResolver();

            // Act
            var response = await registerUserResolver.ExecuteAsync(
                dbContext, _localizer, _emailService,
                email, password, firstName, lastName, patronymic);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("A user with this email already exists", response.ErrorMessage);
        }

        private static DbContextOptions<SqlContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<SqlContext>();
            builder.UseInMemoryDatabase("TestDatabase")
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }

}
