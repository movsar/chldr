using domain.DatabaseObjects.Models;
using domain.DatabaseObjects.Dtos;
using sql_dl;
using sql_dl.Services;
using sql_dl.SqlEntities;
using domain.Interfaces.Repositories;
using domain.Models;
using Microsoft.EntityFrameworkCore;
using sql_dl.Interfaces;
using sql_dl;
using sql_dl.Interfaces;
using domain.Interfaces;
using domain;
using domain.Enums;

namespace sql_dl.Repositories
{
    public class SqlUsersRepository : SqlRepository<SqlUser, UserModel, UserDto>, IUsersRepository
    {
        public SqlUsersRepository(
            SqlContext context,
            IFileService fileService,
            string _userId) : base(context, fileService, _userId){}


        protected override RecordType RecordType => RecordType.User;
   
        public async Task SetStatusAsync(string userId, UserStatus newStatus)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new NullReferenceException();
            }
            user.Status = (int)newStatus;

            await _dbContext.SaveChangesAsync();
        }

        public override async Task<List<ChangeSetModel>> AddAsync(UserDto dto)
        {
            dto.Rate = 1;

            var user = SqlUser.FromDto(dto);
            await _dbContext.Users.AddAsync(user);

            var changeSet = CreateChangeSetEntity(Operation.Insert, dto.Id);
            await _dbContext.ChangeSets.AddAsync(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> UpdateAsync(UserDto dto)
        {
            var existingEntity = await GetAsync(dto.Id);
            var existingDto = UserDto.FromModel(existingEntity);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                return new List<ChangeSetModel>();
            }

            var user = SqlUser.FromDto(dto);
            _dbContext.Users.Update(user);

            var changeSet = CreateChangeSetEntity(Operation.Update, dto.Id, changes);
            _dbContext.ChangeSets.Add(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public async Task<UserModel?> FindByEmail(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email!.Equals(email));
            return user == null ? null : UserModel.FromEntity(user);
        }
      
        public override async Task<List<UserModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();
            var ids = await _dbContext.Set<SqlUser>().Select(e => e.Id).ToListAsync();
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            var entities = await _dbContext.Set<SqlUser>()
              .Where(e => randomlySelectedIds.Contains(e.Id))
              .AsNoTracking()
              .ToListAsync();

            var models = entities.Select(UserModel.FromEntity).ToList();
            return models;
        }

        public async Task<UserModel> GetByEmailAsync(string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException();
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email!.ToLower().Equals(email.ToLower()));
            if (user == null)
            {
                throw new NullReferenceException("No such user");
            }
            return UserModel.FromEntity(user);
        }

        public override async Task<UserModel> GetAsync(string entityId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id!.Equals(entityId));
            return UserModel.FromEntity(user);
        }

        public override async Task<List<UserModel>> TakeAsync(int offset, int limit)
        {
            var entities = await _dbContext.Users
                .Skip(offset)
                .Take(limit)
                .Cast<SqlUser>()
                .ToListAsync();

            return entities.Select(UserModel.FromEntity).ToList();
        }
    }
}
