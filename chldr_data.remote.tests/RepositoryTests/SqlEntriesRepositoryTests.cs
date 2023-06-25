using chldr_data.remote.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace chldr_data.remote.tests.RepositoryTests
{
    public class SqlEntriesRepositoryTests
    {
        private readonly SqlContext _dbContext;
        private readonly SqlEntriesRepository _repository;
        private readonly string _userId;

        public SqlEntriesRepositoryTests()
        {
            // Create an in-memory SQLite database for testing
            var options = new DbContextOptionsBuilder<SqlContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new SqlContext(options);

            // Mock any dependencies required by the repository
            var fileServiceMock = new Mock<FileService>();
            _userId = "test_user";

            // Create the repository instance
            _repository = new SqlEntriesRepository(_dbContext, fileServiceMock.Object, _userId);
        }

        [Fact]
        public void Get_ExistingEntity_ReturnsEntity()
        {
            // Arrange
            var entityId = "existing_entity_id";
            var entity = new SqlEntry { EntryId = entityId, Content = "randomword", Type = 1 };
            _dbContext.Entries.Add(entity);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Get(entityId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entityId, result.EntryId);
        }

        [Fact]
        public void Get_NonExistingEntity_ThrowsException()
        {
            // Arrange
            var entityId = "non_existing_entity_id";

            // Act and Assert
            Assert.Throws<Exception>(() => _repository.Get(entityId));
        }

        [Fact]
        public void Remove_ExistingEntity_RemovesEntity()
        {
            // Arrange
            var entityId = "existing_entity_id";
            var entity = new SqlEntry { EntryId = entityId, Content = "randomword", Type = 1 };
            _dbContext.Entries.Add(entity);
            _dbContext.SaveChanges();

            // Act
            _repository.Remove(entityId);

            // Assert
            var result = _dbContext.Entries.Find(entityId);
            Assert.Null(result);
        }

        [Fact]
        public void Remove_NonExistingEntity_DoesNothing()
        {
            // Arrange
            var entityId = "non_existing_entity_id";

            // Act
            _repository.Remove(entityId);

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
            var result = await _repository.TakeAsync(offset, limit);

            // Assert
            Assert.Equal(limit, result.Count());
            Assert.Equal(entities.Take(limit).Select(e => e.EntryId), result.Select(e => e.EntryId));
        }
    }
}