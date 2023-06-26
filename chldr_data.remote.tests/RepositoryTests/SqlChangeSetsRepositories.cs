using chldr_data.local.Services;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Services;
using global::chldr_data.remote.Repositories;
using global::chldr_data.remote.Services;
using global::chldr_data.remote.SqlEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace chldr_data.remote.tests.RepositoryTests
{
    public class SqlChangeSetsRepositoryTests
    {
        private SqlDataProvider _dataProvider;

        private SqlDataProvider CreateInMemoryDataProvider()
        {
            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<SqlContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var fileService = new FileService();
            var exceptionHandler = new ExceptionHandler();

            var sqlContext = new SqlContext(options);

            return new SqlDataProvider(fileService, null, exceptionHandler);
        }
        public SqlChangeSetsRepositoryTests()
        {
        }

        [Fact]
        public async Task TakeLastAsync_ReturnsCorrectNumberOfChangeSets()
        {
            var tasks = Enumerable.Range(0, 5).Select(async _ =>
            {
                // Act
                var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork();
                var result = await unitOfWork.ChangeSets.TakeLastAsync(1);

                // Assert
                Assert.Single(result);
            });

            await Task.WhenAll(tasks);
        }

    }
}