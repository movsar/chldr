using api_server.GraphQL.MutationServices;
using core.DatabaseObjects.Dtos;
using core.Enums;
using core.Interfaces;
using core.Models;
using chldr_test_utils;
using Newtonsoft.Json;

namespace api_server.tests.ServiceResolverTests
{
    public class UserResolverTests
    {
        private UserResolver _userResolver;
        private IDataProvider _testDataProvider;

        public UserResolverTests()
        {
            // ! You shouldn't use unitOfWork in the tests at all!
          
            _testDataProvider = TestDataFactory.CreateSqliteDataProvider();
            _userResolver = TestDataFactory.CreateFakeUserResolver(_testDataProvider);
        }

        [Fact]
        public async Task RegisterAndLogIn_WithValidInput_ReturnsSuccess()
        {
            // Arrange
            var newUserInfo = TestDataFactory.CreateRandomUserDto();

            // Act
            var registrationResponse = await _userResolver.RegisterAndLogInAsync(newUserInfo.Email!, newUserInfo.Password, newUserInfo.FirstName, newUserInfo.LastName, newUserInfo.Patronymic);
          
            // Assert
            Assert.True(registrationResponse.Success);

            var data = RequestResult.GetData<dynamic>(registrationResponse);
            var sessionInformation = new SessionInformation()
            {
                AccessToken = data.AccessToken!,
                RefreshToken = data.RefreshToken!,
                Status = SessionStatus.LoggedIn,
                UserDto = JsonConvert.DeserializeObject<UserDto>(data.User.ToString())
            };

            Assert.NotEmpty(sessionInformation.AccessToken);
            Assert.NotEmpty(sessionInformation.RefreshToken);
            Assert.Equal(sessionInformation.UserDto.Email, newUserInfo.Email);
        }

        [Fact]
        public async Task RegisterAndLogIn_WithExistingUser_ReturnsError()
        {
            // Arrange
            var newUserInfo = TestDataFactory.CreateRandomUserDto();

            // Act
            var registrationResponse1 = await _userResolver.RegisterAndLogInAsync(newUserInfo.Email!, newUserInfo.Password, newUserInfo.FirstName, newUserInfo.LastName, newUserInfo.Patronymic);
            var registrationResponse2 = await _userResolver.RegisterAndLogInAsync(newUserInfo.Email!, newUserInfo.Password, newUserInfo.FirstName, newUserInfo.LastName, newUserInfo.Patronymic);
          
            // Assert
            Assert.True(registrationResponse1.Success);
            Assert.False(registrationResponse2.Success);           
        }
    }
}
