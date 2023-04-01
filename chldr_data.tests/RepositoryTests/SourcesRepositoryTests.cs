using chldr_data.Repositories;

namespace chldr_data.tests.RepositoryTests
{
    public class SourcesRepositoryTests : TestsBase
    {
        [Fact]
        public void GetUnverifiedSources_NoInput_ReturnsSources()
        {
            var UnverifiedSources = SourcesRepository.GetUnverifiedSources();
            Assert.True(UnverifiedSources.Count() > 0);
        }
    }
}