﻿using chldr_api.GraphQL.MutationServices;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Repositories;
using chldr_data.remote.Services;
using chldr_data.Resources.Localizations;
using chldr_data.Services;
using chldr_test_utils;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.Extensions.Localization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using System.Transactions;

namespace chldr_api.tests.ServiceResolverTests
{
    public class UserResolverTests
    {
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly EmailService _emailService;
        private readonly IDataProvider _dataProvider;
        private readonly FileService _fileService;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly EnvironmentService _environmentService;
        private readonly UserResolver _userResolver;

        public UserResolverTests()
        {
            _emailService = TestDataFactory.CreateFakeEmailService();
            _localizer = TestDataFactory.GetStringLocalizer();
            _dataProvider = TestDataFactory.CreateSqlDataProvider();
            _fileService = new FileService(AppContext.BaseDirectory);
            _exceptionHandler = new ExceptionHandler(_fileService);
            _environmentService = new EnvironmentService(chldr_shared.Enums.Platforms.Web, true);
            _userResolver = new UserResolver(_dataProvider, _localizer, _emailService, _exceptionHandler, _fileService);
        }

        [Fact]
        public async Task Confirm_WithValidInput_ReturnsSuccessResponse()
        {
            // Arrange
            var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
            unitOfWork.BeginTransaction();

            var usersRepository = (SqlUsersRepository)unitOfWork.Users;
            var tokensRepository = (SqlTokensRepository)unitOfWork.Tokens;
            var newUserInfo = TestDataFactory.CreateRandomUserDto();

            // Act and Assert
            var registrationResponse = await _userResolver.Register(newUserInfo.Email!, newUserInfo.Password, newUserInfo.FirstName, newUserInfo.LastName, newUserInfo.Patronymic);
            Assert.True(registrationResponse.Success);

            var confirmationToken = JsonConvert.DeserializeObject<string>(registrationResponse.SerializedData);
            Assert.NotEmpty(confirmationToken);

            var confirmationResponse = await _userResolver.Confirm(confirmationToken);
            Assert.True(confirmationResponse.Success);

            UserModel user = await usersRepository.GetByEmailAsync(newUserInfo.Email);
            Assert.NotNull(user);
            Assert.Equal(UserStatus.Active, user.Status);
            unitOfWork.Rollback();
        }

        [Fact]
        public async Task RegisterNew_WithValidInput_ReturnsSuccessResponse()
        {
            // Arrange
            var newUser = TestDataFactory.CreateRandomUserDto();

            // Act
            var response = await _userResolver.Register(newUser.Email!, newUser.Password, newUser.FirstName, newUser.LastName, newUser.Patronymic);

            // Assert
            Assert.True(response.Success);

            var token = JsonConvert.DeserializeObject(response.SerializedData!);

            Assert.NotNull(token);
        }

        [Fact]
        public async Task RegisterNew_WithExistingEmail_ReturnsErrorResponse()
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var actingUserId = unitOfWork.Users.GetRandomsAsync(1).Result.First().UserId;

            // Arrange
            var testUser = TestDataFactory.CreateRandomUserDto();
            unitOfWork = _dataProvider.CreateUnitOfWork(actingUserId);
            await unitOfWork.Users.Add(testUser);

            // Act
            var response = await _userResolver.Register(testUser.Email!, testUser.Password, testUser.FirstName, testUser.LastName, testUser.Patronymic);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("A user with this email already exists", response.ErrorMessage);
        }

    }

}