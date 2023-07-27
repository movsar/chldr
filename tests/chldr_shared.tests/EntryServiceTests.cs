using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Interfaces;
using chldr_shared.Services;
using chldr_test_utils;
using System.Transactions;

namespace chldr_shared.tests
{
    public class EntryServiceTests
    {
        private IDataProvider _dataProvider;
        private EntryService _entryService;

        public EntryServiceTests()
        {
            _dataProvider = TestDataFactory.CreateSqlDataProvider();
            _entryService = new EntryService(_dataProvider);
        }

        [Fact]
        public async Task AddEntry_ActiveMember_Success()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {   
                // Arrange
                var unitOfWork = _dataProvider.CreateUnitOfWork();

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
            }
        }

        [Fact]
        public void AddEntry_BannedUser_Fails()
        {

        }

        [Fact]
        public void AddEntry_ActiveEnthusiastEditor_Success()
        {

        }


        //[Fact]
        //public async Task Get_ExistingEntity_ReturnsEntity()
        //{
        //    // Arrange
        //    var entityId = "existing_entity_id";
        //    var entity = new SqlEntry { EntryId = entityId, Content = "randomword", Type = 1 };
        //    _dbContext.Entries.Add(entity);
        //    _dbContext.SaveChanges();

        //    // Act
        //    var result = await _entries.Get(entityId);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(entityId, result.EntryId);
        //}

        //[Fact]
        //public async Task Get_NonExistingEntity_ThrowsException()
        //{
        //    // Arrange
        //    var entityId = "non_existing_entity_id";

        //    // Act and Assert
        //    //Assert.Throws<Exception>(_entries.Get(entityId));
        //}

        //[Fact]
        //public async Task Remove_ExistingEntity_RemovesEntity()
        //{
        //    // Arrange
        //    var entityId = "existing_entity_id";
        //    var entity = new SqlEntry { EntryId = entityId, Content = "randomword", Type = 1 };
        //    _dbContext.Entries.Add(entity);
        //    _dbContext.SaveChanges();

        //    // Act
        //    await _entries.Remove(entityId, null);

        //    // Assert
        //    var result = _dbContext.Entries.Find(entityId);
        //    Assert.Null(result);
        //}

        //[Fact]
        //public async Task Remove_NonExistingEntity_DoesNothing()
        //{
        //    // Arrange
        //    var entityId = "non_existing_entity_id";

        //    // Act
        //    await _entries.Remove(entityId, null);

        //    // Assert
        //    // No exception should be thrown
        //}

        //[Fact]
        //public async Task TakeAsync_ReturnsCorrectEntities()
        //{
        //    // Arrange
        //    var offset = 0;
        //    var limit = 5;
        //    var entities = new List<SqlEntry>
        //    {
        //        new SqlEntry { EntryId = "1", Content = "randomword1", Type = 1 },
        //        new SqlEntry { EntryId = "2", Content = "randomword2", Type = 1 },
        //        new SqlEntry { EntryId = "3", Content = "randomword3", Type = 1 },
        //        new SqlEntry { EntryId = "4", Content = "randomword4", Type = 1 },
        //        new SqlEntry { EntryId = "5", Content = "randomword5", Type = 1 }
        //    };
        //    _dbContext.Entries.AddRange(entities);
        //    _dbContext.SaveChanges();

        //    // Act
        //    var result = await _entries.TakeAsync(offset, limit);

        //    // Assert
        //    Assert.Equal(limit, result.Count());
        //    Assert.Equal(entities.Take(limit).Select(e => e.EntryId), result.Select(e => e.EntryId));
        //}
    }
}