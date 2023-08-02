using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_shared.Services;
using chldr_test_utils;
using chldr_utils.Exceptions;
using System.Reactive;
using System.Transactions;

namespace chldr_shared.tests
{
    public class EntryServiceTests
    {
        [Fact]
        public async Task AddRemoveEntry_ActiveMember_Success()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var dataProvider = TestDataFactory.CreateSqlDataProvider();
                var entryService = new EntryService(dataProvider);

                var unitOfWork = dataProvider.CreateUnitOfWork();
                var actingUserId = unitOfWork.Users.GetRandomsAsync(1).Result.First().UserId;

                // Arrange

                var userDto = TestDataFactory.CreateRandomUserDto();
                userDto.Status = chldr_data.Enums.UserStatus.Active;
                await unitOfWork.Users.Add(userDto);
                unitOfWork = dataProvider.CreateUnitOfWork(userDto.UserId);
                var user = UserModel.FromDto(userDto);

                var source = (await unitOfWork.Sources.GetRandomsAsync(1)).First();

                var entryDto = TestDataFactory.CreateRandomEntryDto(user.UserId, source.SourceId);

                // Act
                await entryService.AddAsync(entryDto, userDto.UserId);

                // Assert
                var insertedEntry = await entryService.GetAsync(entryDto.EntryId);
                var userRateRange = user.GetRateRange();

                Assert.Equal(entryDto.Content, insertedEntry.Content);
                Assert.Equal(userRateRange.Lower, insertedEntry.Rate);
                Assert.Equal(entryDto.SoundDtos[0].SoundId, insertedEntry.Sounds[0].SoundId);

                await entryService.RemoveAsync(entryDto.EntryId, userDto.UserId);
                await Assert.ThrowsAsync<ArgumentException>(async () => await entryService.GetAsync(entryDto.EntryId));
            }
        }

        [Fact]
        public async Task AddEntry_InActiveUser_Fails()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var dataProvider = TestDataFactory.CreateSqlDataProvider();
                var entryService = new EntryService(dataProvider);

                var unitOfWork = dataProvider.CreateUnitOfWork();
                var actingUserId = unitOfWork.Users.GetRandomsAsync(1).Result.First().UserId;
                
                // Arrange
                var user = TestDataFactory.CreateRandomUserDto();
                user.Status = chldr_data.Enums.UserStatus.Banned;
                await unitOfWork.Users.Add(user);

                var source = (await unitOfWork.Sources.GetRandomsAsync(1)).First();
                var entryDto = TestDataFactory.CreateRandomEntryDto(user.UserId, source.SourceId);

                // Act
                await Assert.ThrowsAsync<UnauthorizedException>(async () => await entryService.AddAsync(entryDto, user.UserId));
            }
        }

        [Fact]
        public async Task Get_NonExistingEntity_ThrowsException()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var dataProvider = TestDataFactory.CreateSqlDataProvider();
                var entryService = new EntryService(dataProvider);

                // Arrange
                var entityId = "non_existing_entity_id";

                // Act and Assert
                await Assert.ThrowsAsync<ArgumentException>(async () => await entryService.GetAsync(entityId));
            }
        }

        //[Fact]
        //public async Task AddUser_InvalidRole_SetsMember()
        //{
        //    // Arrange
        //    var unitOfWork = _dataProvider.CreateUnitOfWork();
        //    var actingUser = (await unitOfWork.Users.GetRandomsAsync(1)).First();
        //    unitOfWork = _dataProvider.CreateUnitOfWork(actingUser.UserId);

        //    // Act
        //    var user = TestDataFactory.CreateRandomUserDto();
        //    user.Rate = UserModel.EnthusiastRateRange.Lower + 1;
        //    await unitOfWork.Users.Add(user);

        //    // Assert
        //    var insertedUser = await _userService.Get(user.UserId);
        //    Assert.Equal(UserModel.MemberRateRange.Lower, insertedUser.Rate);
        //}
    }
}