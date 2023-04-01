using chldr_data.tests.Services;

namespace chldr_data.tests.RepositoryTests
{
    public class UsersRepositoryTests : TestsBase
    {
        [Fact]
        public void GetUserByEmail_ExistingUserEmail_ReturnsUser()
        {
            var user = UsersRepository.GetUserByEmail("movsar.dev@gmail.com");
            Assert.NotNull(user);
        }
    }
}
