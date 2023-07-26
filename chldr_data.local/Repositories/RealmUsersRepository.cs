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
using chldr_data.Services;

namespace chldr_data.Repositories
{
    public class RealmUsersRepository : RealmRepository<RealmUser, UserModel, UserDto>, IUsersRepository
    {
        public RealmUsersRepository(ExceptionHandler exceptionHandler, FileService fileService, RequestService requestService, string userId) : base(exceptionHandler, fileService, requestService, userId) { }

        protected override RecordType RecordType => RecordType.User;
        protected override UserModel FromEntityShortcut(RealmUser entity)
        {
            return UserModel.FromEntity(entity);
        }
        public override async Task<List<ChangeSetModel>> Add(UserDto dto, string userId)
        {
            _dbContext.Write(() =>
            {
                var entity = RealmUser.FromDto(dto, _dbContext);
                _dbContext.Add(entity);
            });

            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }

        public override async Task<List<ChangeSetModel>> Update(UserDto dto, string userId)
        {
            var existingEntity = await Get(dto.UserId);
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

        public override Task<List<ChangeSetModel>> Remove(string entityId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
