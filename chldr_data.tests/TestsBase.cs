
using chldr_data.Repositories;
using chldr_data.Interfaces;
using chldr_data.tests.Services;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.tests
{
    public class TestsBase
    {
        public TestsBase()
        {
            _localDbReader = TestDataFactory.GetTestDataAccess();
            _localDbReader.RemoveAllEntries();
        }

        protected ILocalDbReader _localDbReader;
    }
}
