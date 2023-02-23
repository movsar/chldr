using chldr_data.Interfaces;
using chldr_data.tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.tests.RepositoryTests
{
    public class LanguagesRepositoryTests
    {
        private readonly IDataAccess _dataAccess;
        public LanguagesRepositoryTests()
        {
            _dataAccess = TestDataFactory.GetTestDataAccess();
            _dataAccess.RemoveAllEntries();
        }

        [Fact]
        public void GetAllLanguages_NoInput_ReturnsListOfLanguages()
        {
            var allLanguages = _dataAccess.LanguagesRepository.GetAllLanguages();
            Assert.True(allLanguages.Count() > 0);
        }

    }
}
