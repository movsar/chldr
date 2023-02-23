using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Services;
using chldr_utils.Services;
using chldr_utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chldr_data.tests.Services;

namespace chldr_data.tests.RepositoryTests
{
    public class SourcesRepositoryTests
    {
        private static IDataAccess _dataAccess;

        public SourcesRepositoryTests()
        {
            _dataAccess = TestDataFactory.GetTestDataAccess();
            _dataAccess.RemoveAllEntries();
        }

        [Fact]
        public void GetUnverifiedSources_NoInput_ReturnsSources()
        {
            var UnverifiedSources = _dataAccess.SourcesRepository.GetUnverifiedSources();
            Assert.True(UnverifiedSources.Count() > 0);
        }
    }
}