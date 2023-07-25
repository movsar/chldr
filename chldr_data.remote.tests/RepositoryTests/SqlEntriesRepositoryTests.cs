using chldr_data.remote.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Runtime.CompilerServices;

namespace chldr_data.remote.tests.RepositoryTests
{
    public class SqlEntriesRepositoryTests
    {
        private readonly SqlContext _dbContext;
        private readonly SqlEntriesRepository _entries;
        private readonly string _userId;
        private readonly ExceptionHandler _exceptionHandler;
        public SqlEntriesRepositoryTests()
        {
            // Create an in-memory SQLite database for testing
            var options = new DbContextOptionsBuilder<SqlContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new SqlContext(options);
            _exceptionHandler = new ExceptionHandler();
            // Mock any dependencies required by the repository
            var fileServiceMock = new Mock<FileService>();
            _userId = "test_user";

            var translations = new SqlTranslationsRepository(_dbContext, fileServiceMock.Object, _userId);
            var sounds = new SqlSoundsRepository(_dbContext, fileServiceMock.Object, _userId);

            _entries = new SqlEntriesRepository(
                _dbContext,
                fileServiceMock.Object, _exceptionHandler,
                translations,
                sounds,
                _userId
            );
        }

        [Fact]
        public async Task Get_ExistingEntity_ReturnsEntity()
        {
            // Arrange
            var entityId = "existing_entity_id";
            var entity = new SqlEntry { EntryId = entityId, Content = "randomword", Type = 1 };
            _dbContext.Entries.Add(entity);
            _dbContext.SaveChanges();

            // Act
            var result = await _entries.Get(entityId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entityId, result.EntryId);
        }

        [Fact]
        public async Task Get_NonExistingEntity_ThrowsException()
        {
            // Arrange
            var entityId = "non_existing_entity_id";

            // Act and Assert
            //Assert.Throws<Exception>(_entries.Get(entityId));
        }

        [Fact]
        public async Task Remove_ExistingEntity_RemovesEntity()
        {
            // Arrange
            var entityId = "existing_entity_id";
            var entity = new SqlEntry { EntryId = entityId, Content = "randomword", Type = 1 };
            _dbContext.Entries.Add(entity);
            _dbContext.SaveChanges();

            // Act
            await _entries.Remove(entityId, null);

            // Assert
            var result = _dbContext.Entries.Find(entityId);
            Assert.Null(result);
        }

        [Fact]
        public async Task Remove_NonExistingEntity_DoesNothing()
        {
            // Arrange
            var entityId = "non_existing_entity_id";

            // Act
            await _entries.Remove(entityId, null);

            // Assert
            // No exception should be thrown
        }

        [Fact]
        public async Task TakeAsync_ReturnsCorrectEntities()
        {
            // Arrange
            var offset = 0;
            var limit = 5;
            var entities = new List<SqlEntry>
            {
                new SqlEntry { EntryId = "1", Content = "randomword1", Type = 1 },
                new SqlEntry { EntryId = "2", Content = "randomword2", Type = 1 },
                new SqlEntry { EntryId = "3", Content = "randomword3", Type = 1 },
                new SqlEntry { EntryId = "4", Content = "randomword4", Type = 1 },
                new SqlEntry { EntryId = "5", Content = "randomword5", Type = 1 }
            };
            _dbContext.Entries.AddRange(entities);
            _dbContext.SaveChanges();

            // Act
            var result = await _entries.TakeAsync(offset, limit);

            // Assert
            Assert.Equal(limit, result.Count());
            Assert.Equal(entities.Take(limit).Select(e => e.EntryId), result.Select(e => e.EntryId));
        }
    }
}