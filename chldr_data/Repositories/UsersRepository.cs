using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Models;

namespace chldr_data.Repositories
{
    public class UsersRepository : Repository
    {
        public UsersRepository(IDataAccess dataAccess) : base(dataAccess) { }
        public UserModel GetUserByEmail(string email)
        {
            var user = Database.All<User>().Where(u => u.Email == email).FirstOrDefault();
            return new UserModel(user);
        }
    }
}
