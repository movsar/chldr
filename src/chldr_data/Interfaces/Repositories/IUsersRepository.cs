using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface IUsersRepository : IRepository<UserModel, UserDto>
    {
    }
}
