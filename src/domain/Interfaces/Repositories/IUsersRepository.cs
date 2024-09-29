using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Models;

namespace domain.Interfaces.Repositories
{
    public interface IUsersRepository : IRepository<UserModel, UserDto>
    {
    }
}
