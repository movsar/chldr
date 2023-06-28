using chldr_data.local.Services;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Services;

namespace chldr_data.remote.tests.RepositoryTests
{
    public class SqlChangeSetsRepositoryTests
    {
        private SqlDataProvider _dataProvider;

        private SqlDataProvider CreateInMemoryDataProvider()
        {
            var fileService = new FileService();
            var exceptionHandler = new ExceptionHandler();

            return new SqlDataProvider(fileService, exceptionHandler, Constants.TestDatabaseConnectionString);
        }
        public SqlChangeSetsRepositoryTests()
        {
            _dataProvider = CreateInMemoryDataProvider();
        }

        [Fact]
        public async Task TakeLastAsync_ReturnsCorrectNumberOfChangeSets()
        {
            var tasks = Enumerable.Range(0, 1).Select(async _ =>
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