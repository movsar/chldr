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
        public RealmUsersRepository(ExceptionHandler exceptionHandler, FileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }

        protected override RecordType RecordType => RecordType.User;
        public override UserModel FromEntity(RealmUser entity)
        {
            return UserModel.FromEntity(entity);
        }
        public override async Task<List<ChangeSetModel>> Add(UserDto dto)
        {
            _dbContext.Write(() =>
            {
                var entity = RealmUser.FromDto(dto, _dbContext);
                _dbContext.Add(entity);
            });

            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }

        public override async Task<List<ChangeSetModel>> Update(UserDto dto)
        {
            var existingEntity = await GetAsync(dto.UserId);
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

        public override Task<List<ChangeSetModel>> Remove(string entityId)
        {
            throw new NotImplementedException();
        }
    }
}
