using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_tools;
using chldr_data.Enums;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Interfaces.Repositories;

namespace chldr_data.remote.Repositories
{
    internal class SqlUsersRepository : SqlRepository<UserModel, UserDto>, IUsersRepository
    {
        public SqlUsersRepository(SqlContext context, string _userId) : base(context, _userId) { }

        protected override RecordType RecordType => RecordType.User;

        public override UserModel Get(string entityId)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<UserModel> Take(int limit)
        {
            throw new NotImplementedException();
        }

        public override void Update(UserDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Insert(UserDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string entityId)
        {
            throw new NotImplementedException();
        }
    }
}
