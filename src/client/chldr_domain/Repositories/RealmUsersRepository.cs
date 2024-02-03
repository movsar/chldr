using core.DatabaseObjects.Models;
using core.DatabaseObjects.Dtos;
using core.Enums;
using chldr_domain.RealmEntities;
using core.Interfaces.Repositories;
using core.Models;
using core.Interfaces;

namespace core.Repositories
{
    public class RealmUsersRepository : RealmRepository<RealmUser, UserModel, UserDto>, IUsersRepository
    {
        public RealmUsersRepository(IExceptionHandler exceptionHandler, IFileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }

        protected override RecordType RecordType => RecordType.User;
        public override UserModel FromEntity(RealmUser entity)
        {
            return UserModel.FromEntity(entity);
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task<List<ChangeSetModel>> Add(UserDto dto)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
            var existingEntity = await GetAsync(dto.Id);
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
