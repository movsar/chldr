using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;
using chldr_shared.Services;
using chldr_test_utils;
using System.Reactive;
using System.Transactions;

namespace chldr_shared.tests
{
    public class EntryServiceTests
    {
        private IDataProvider _dataProvider;
        private EntryService _entryService;
        private static string _userId;

        public EntryServiceTests()
        {
            _dataProvider = TestDataFactory.CreateSqlDataProvider();
            _entryService = new EntryService(_dataProvider);

            var unitOfWork = _dataProvider.CreateUnitOfWork();
            _userId = unitOfWork.Users.GetRandomsAsync(1).Result.First().UserId;
        }

        [Fact]
        public async Task AddRemoveEntry_ActiveMember_Success()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Arrange
                var unitOfWork = _dataProvider.CreateUnitOfWork(_userId);

                var user = (await unitOfWork.Users.GetRandomsAsync(1)).First();
                var source = (await unitOfWork.Sources.GetRandomsAsync(1)).First();

                var entryDto = TestDataFactory.CreateRandomEntryDto(user.UserId, source.SourceId);

                // Act
                await _entryService.AddEntry(entryDto, user.UserId);

                // Assert
                var insertedEntry = await _entryService.Get(entryDto.EntryId);
                var userRateRange = user.GetRateRange();

                Assert.Equal(entryDto.Content, insertedEntry.Content);
                Assert.Equal(userRateRange.Lower, insertedEntry.Rate);
                Assert.Equal(entryDto.Sounds[0].SoundId, insertedEntry.Sounds[0].SoundId);

                await _entryService.Remove(entryDto.EntryId, user.UserId);
                await Assert.ThrowsAsync<ArgumentException>(async () => await _entryService.Get(entryDto.EntryId));
            }
        }

        [Fact]
        public async Task AddEntry_InActiveUser_Fails()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Arrange
                var unitOfWork = _dataProvider.CreateUnitOfWork(_userId);

                var user = TestDataFactory.CreateRandomUserDto();
                user.Status = chldr_data.Enums.UserStatus.Banned;
                await unitOfWork.Users.Add(user);

                var source = (await unitOfWork.Sources.GetRandomsAsync(1)).First();
                var entryDto = TestDataFactory.CreateRandomEntryDto(user.UserId, source.SourceId);

                // Act
                await Assert.ThrowsAsync<InvalidOperationException>(async () => await _entryService.AddEntry(entryDto, user.UserId));
            }
        }

        [Fact]
        public async Task Get_NonExistingEntity_ThrowsException()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Arrange
                var entityId = "non_existing_entity_id";

                // Act and Assert
                await Assert.ThrowsAsync<ArgumentException>(async () => await _entryService.Get(entityId));
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