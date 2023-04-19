using chldr_data.Dto;
using chldr_data.Repositories;

namespace chldr_data.tests.RepositoryTests
{
    public class SourcesRepositoryTests : TestsBase
    {
        [Fact]
        public void GetUnverifiedSources_NoInput_ReturnsSources()
        {
            // Arrange
            var sourceDto = new SourceDto()
            {
                Name = "New Source"
            };

            // Insert
            var insertedSourceId = SourcesRepository.Insert(sourceDto);

            // Check
            Assert.NotEqual(string.Empty, insertedSourceId);
        }

        [Fact]
        public void Insert_ExpectedInput_ReturnsId()
        {
            var UnverifiedSources = SourcesRepository.GetUnverifiedSources();
            Assert.True(UnverifiedSources.Count() > 0);
        }
    }
}