using chldr_api.GraphQL.MutationServices;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Resources.Localizations;
using chldr_data.Services;
using chldr_test_utils;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using System.Transactions;

namespace chldr_api.tests.ServiceResolverTests
{
    public class UserResolverTests
    {
        private UserResolver _userResolver;
        private IDataProvider _testDataProvider;

        public UserResolverTests()
        {
            _testDataProvider = TestDataFactory.CreateSqliteDataProvider();
            _userResolver = TestDataFactory.CreateFakeUserResolver(_testDataProvider);
        }

        [Fact]
        public async Task Confirm_WithValidInput_ReturnsSuccessResponse()
        {
            // ! You shouldn't use unitOfWork in the tests at all!

            // Arrange
            var newUserInfo = TestDataFactory.CreateRandomUserDto();

            // Act and Assert
            var registrationResponse = await _userResolver.RegisterAndLogInAsync(newUserInfo.Email!, newUserInfo.Password, newUserInfo.FirstName, newUserInfo.LastName, newUserInfo.Patronymic);
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
        public async Task RegisterNew_WithValidInput_ReturnsSuccessResponse()
        {
            // Arrange
            var newUser = TestDataFactory.CreateRandomUserDto();

            // Act
            var response = await _userResolver.RegisterAndLogInAsync(newUser.Email!, newUser.Password, newUser.FirstName, newUser.LastName, newUser.Patronymic);

            // Assert
            Assert.True(response.Success);

            var token = JsonConvert.DeserializeObject(response.SerializedData!);

            Assert.NotNull(token);
        }

        [Fact]
        public async Task RegisterNew_WithExistingEmail_ReturnsErrorResponse()
        {
            var unitOfWork = _testDataProvider.CreateUnitOfWork();
            var actingUserId = unitOfWork.Users.GetRandomsAsync(1).Result.First().Id;

            // Arrange
            var testUser = TestDataFactory.CreateRandomUserDto();
            unitOfWork = _testDataProvider.CreateUnitOfWork(actingUserId);
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;
            await usersRepository.AddAsync(testUser);

            // Act
            var response = await _userResolver.RegisterAndLogInAsync(testUser.Email!, testUser.Password, testUser.FirstName, testUser.LastName, testUser.Patronymic);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("A user with this email already exists", response.ErrorMessage);
        }

    }

}
