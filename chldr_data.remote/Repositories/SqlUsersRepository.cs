using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_tools;
using chldr_data.Enums;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Services;

namespace chldr_data.remote.Repositories
{
    internal class SqlUsersRepository : SqlRepository<SqlUser, UserModel, UserDto>, IUsersRepository
    {
        public SqlUsersRepository(SqlContext context, FileService fileService, string _userId) : base(context, fileService, _userId) { }

        protected override RecordType RecordType => RecordType.User;

        public override void Add(UserDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Update(UserDto dto)
        {
            throw new NotImplementedException();
        }

        protected override UserModel FromEntityShortcut(SqlUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
