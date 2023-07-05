using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_tools;
using chldr_data.Enums;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Services;
using chldr_data.Models;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.remote.Repositories
{
    public class SqlUsersRepository : SqlRepository<SqlUser, UserModel, UserDto>, IUsersRepository
    {
        public SqlUsersRepository(SqlContext context, FileService fileService, string _userId) : base(context, fileService, _userId) { }

        protected override RecordType RecordType => RecordType.User;
        public async Task SetStatus(string userId, UserStatus newStatus)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new NullReferenceException();
            }

            user.Status = (int)newStatus;
            await _dbContext.SaveChangesAsync();
        }

        public override List<ChangeSetModel> Add(UserDto dto)
        {
            var user = SqlUser.FromDto(dto);
            _dbContext.Users.Add(user);

            var changeSet = CreateChangeSetEntity(Operation.Insert, dto.UserId);
            _dbContext.ChangeSets.Add(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override List<ChangeSetModel> Update(UserDto dto)
        {
            var existingEntity = Get(dto.UserId);
            var existingDto = UserDto.FromModel(existingEntity);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                return new List<ChangeSetModel>();
            }

            var user = SqlUser.FromDto(dto);
            _dbContext.Users.Update(user);

            var changeSet = CreateChangeSetEntity(Operation.Update, dto.UserId, changes);
            _dbContext.ChangeSets.Add(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        protected override UserModel FromEntityShortcut(SqlUser entity)
        {
            return UserModel.FromEntity(entity);
        }

        public async Task<UserModel?> FindByEmail(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email!.Equals(email));
            return user == null ? null : UserModel.FromEntity(user);
        }
    }
}
