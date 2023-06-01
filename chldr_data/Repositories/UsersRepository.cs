using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.RealmEntities;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.DatabaseObjects.Dtos;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class UsersRepository : Repository<SqlUser, UserModel, UserDto>
    {
        public UsersRepository(SqlContext context) : base(context) { }
        public UserModel GetUserByEmail(string email)
        {
            //var user = Database.All<RealmUser>().Where(u => u.Email == email).FirstOrDefault();
            //return UserModel.FromEntity(user);
            return null;
        }
    }
}
