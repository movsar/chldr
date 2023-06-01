using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.RealmEntities;

namespace chldr_data.Repositories
{
    public class UsersRepository : Repository
    {
        public UsersRepository(ILocalDbReader dataAccess) : base(dataAccess) { }
        public UserModel GetUserByEmail(string email)
        {
            var user = Database.All<RealmUser>().Where(u => u.Email == email).FirstOrDefault();
            return UserModel.FromEntity(user);
        }
    }
}
