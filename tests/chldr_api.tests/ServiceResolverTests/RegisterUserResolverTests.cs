using chldr_api.GraphQL.MutationServices;
using chldr_data.Interfaces;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Resources.Localizations;
using chldr_test_utils;
using chldr_utils.Services;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace chldr_api.tests.ServiceResolverTests
{
    public class RegisterUserResolverTests
    {
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly EmailService _emailService;
        private IDataProvider _dataProvider;

        public RegisterUserResolverTests()
        {
            _emailService = TestDataFactory.CreateFakeEmailService();
            _localizer = TestDataFactory.GetStringLocalizer();
            //_dataProvider = TestDataFactory.CreateSqlDataProvider();
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInput_ReturnsSuccessResponse()
        {
            // Arrange
            var newUser = TestDataFactory.CreateRandomUserDto();
            var registerUserResolver = new RegisterUserResolver();

            // Act
            var response = await registerUserResolver.ExecuteAsync(
                (SqlDataProvider)_dataProvider, _localizer, _emailService,
                newUser.Email!, newUser.Password, newUser.FirstName, newUser.LastName, newUser.Patronymic);

            // Assert
            Assert.True(response.Success);

            var token = JsonConvert.DeserializeObject(response.SerializedData!);

            Assert.NotNull(token);
        }

        [Fact]
        public async Task ExecuteAsync_WithExistingEmail_ReturnsErrorResponse()
        {
            // Arrange
            var testUser = TestDataFactory.CreateRandomUserDto();
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            await unitOfWork.Users.Add(testUser);

            var registerUserResolver = new RegisterUserResolver();

            // Act
            var response = await registerUserResolver.ExecuteAsync(
                (SqlDataProvider)_dataProvider, _localizer, _emailService,
                testUser.Email!, testUser.Password, testUser.FirstName, testUser.LastName, testUser.Patronymic);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("A user with this email already exists", response.ErrorMessage);
        }

    }

}
