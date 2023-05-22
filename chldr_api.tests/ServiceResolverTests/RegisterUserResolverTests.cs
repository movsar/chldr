using chldr_api.GraphQL.MutationServices;
using chldr_api.tests.Factories;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Resources.Localizations;
using chldr_utils.Services;
using Microsoft.Extensions.Localization;

namespace chldr_api.tests.ServiceResolverTests
{
    public class RegisterUserResolverTests
    {
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly EmailService _emailService;

        public RegisterUserResolverTests()
        {
            _emailService = TestServiceFactory.CreateTestEmailService();
            _localizer = TestServiceFactory.CreateTestStringLocalizer();
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInput_ReturnsSuccessResponse()
        {
            // Arrange
            using var dbContext = TestServiceFactory.CreateTestDbContext();
            var testUser = TestDataFactory.CreateUser();
            var registerUserResolver = new RegisterUserResolver();

            // Act
            var response = await registerUserResolver.ExecuteAsync(
                dbContext, _localizer, _emailService,
                testUser.Email!, testUser.Password, testUser.FirstName, testUser.LastName, testUser.Patronymic);

            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response.Token);
        }

        [Fact]
        public async Task ExecuteAsync_WithExistingEmail_ReturnsErrorResponse()
        {
            // Arrange
            using var dbContext = TestServiceFactory.CreateTestDbContext();
            var testUser = TestDataFactory.CreateUser();

            // Add existing user to the database
            dbContext.Users.Add(SqlUser.FromDto(testUser));
            await dbContext.SaveChangesAsync();

            var registerUserResolver = new RegisterUserResolver();

            // Act
            var response = await registerUserResolver.ExecuteAsync(
                dbContext, _localizer, _emailService,
                testUser.Email!, testUser.Password, testUser.FirstName, testUser.LastName, testUser.Patronymic);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("A user with this email already exists", response.ErrorMessage);
        }

    }

}
