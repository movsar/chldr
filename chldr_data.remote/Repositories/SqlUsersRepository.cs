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
using Org.BouncyCastle.Crypto.Generators;
using Realms.Sync;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.remote.Repositories
{
    public class SqlUsersRepository : SqlRepository<SqlUser, UserModel, UserDto>, IUsersRepository
    {
        public SqlUsersRepository(DbContextOptions<SqlContext> dbConfig, FileService fileService, string _userId) : base(dbConfig, fileService, _userId) { }

        protected override RecordType RecordType => RecordType.User;
        public async Task SetStatus(string userId, UserStatus newStatus)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new NullReferenceException();
            }
            user.Status = (int)newStatus;

            await _dbContext.SaveChangesAsync();
        }

        public override async Task<List<ChangeSetModel>> Add(UserDto dto)
        {
            dto.Rate = 1;

            var user = SqlUser.FromDto(dto);
            await _dbContext.Users.AddAsync(user);

            var changeSet = CreateChangeSetEntity(Operation.Insert, dto.UserId);
            await _dbContext.ChangeSets.AddAsync(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> Update(UserDto dto)
        {
            var existingEntity = await Get(dto.UserId);
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

        public async Task<bool> VerifyAsync(string userId, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId.Equals(userId));
            if (user == null)
            {
                throw new NullReferenceException();
            }

            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        public override async Task<List<UserModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();
            var ids = await _dbContext.Set<SqlUser>().Select(e => e.UserId).ToListAsync();
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            var entities = await _dbContext.Set<SqlUser>()
              .Where(e => randomlySelectedIds.Contains(e.UserId))
              .AsNoTracking()
              .ToListAsync();

            var models = entities.Select(FromEntityShortcut).ToList();
            return models;
        }
    }
}
