using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.DatabaseObjects.Dtos;
using chldr_tools;
using chldr_data.Enums;

namespace chldr_data.Repositories
{
    public class UsersRepository : Repository<SqlUser, UserModel, UserDto>
    {
        public UsersRepository(SqlContext context) : base(context) { }

        protected override RecordType RecordType => RecordType.User;

        public override IEnumerable<ChangeSetModel> Add(string userId, UserDto dto)
        {
            throw new NotImplementedException();
        }

        public override UserModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, UserDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
