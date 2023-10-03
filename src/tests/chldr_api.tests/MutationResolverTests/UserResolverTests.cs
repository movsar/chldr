﻿using chldr_api.GraphQL.MutationServices;
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
