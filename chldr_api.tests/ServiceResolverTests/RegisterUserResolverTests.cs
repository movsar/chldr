using chldr_api.GraphQL.MutationServices;
using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Resources.Localizations;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
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
            _localizer = new Mock<IStringLocalizer<AppLocalizations>>().Object;
            _emailService = new Mock<EmailService>().Object;
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInput_ReturnsSuccessResponse()
        {
            // Arrange
            var dbContext = CreateDbContext();
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
            var dbContext = CreateDbContext();
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

        private SqlContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<SqlContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new SqlContext(options);
        }
    }

}
