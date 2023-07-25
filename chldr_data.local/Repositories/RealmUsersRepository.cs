using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_tools;
using chldr_data.Enums;
using Realms;
using chldr_data.local.RealmEntities;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.Models;

namespace chldr_data.Repositories
{
    public class RealmUsersRepository : RealmRepository<RealmUser, UserModel, UserDto>, IUsersRepository
    {
        public RealmUsersRepository(ExceptionHandler exceptionHandler, FileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }

        protected override RecordType RecordType => RecordType.User;
        protected override UserModel FromEntityShortcut(RealmUser entity)
        {
            return UserModel.FromEntity(entity);
        }
        public override List<ChangeSetModel> Add(UserDto dto)
        {
            _dbContext.Write(() =>
            {
                var entity = RealmUser.FromDto(dto, _dbContext);
                _dbContext.Add(entity);
            });

            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }

        public override List<ChangeSetModel> Update(UserDto dto)
        {
            var existingEntity = Get(dto.UserId);
            var existingDto = UserDto.FromModel(existingEntity);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                // ! NOT IMPLEMENTED
                return new List<ChangeSetModel>();
            }

            _dbContext.Write(() =>
            {
                var entity = RealmUser.FromDto(dto, _dbContext);
            });

            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }

    }
}
