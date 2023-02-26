using chldr_data.Interfaces;
using chldr_data.tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.tests.RepositoryTests
{
    public class UsersRepositoryTests
    {
        private IDataAccess _dataAccess;

        public UsersRepositoryTests()
        {
            _dataAccess = TestDataFactory.GetTestDataAccess();
            _dataAccess.RemoveAllEntries();
        }

        [Fact]
        public void GetUserByEmail_ExistingUserEmail_ReturnsUser()
        {
            var user = _dataAccess.UsersRepository.GetUserByEmail("movsar.dev@gmail.com");
            Assert.NotNull(user);
        }
    }
}
