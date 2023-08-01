using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using chldr_data.Enums;
using chldr_data.Services;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.SqlEntities;
using chldr_data.remote.Services;
using chldr_data.Models;
using chldr_utils.Services;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.remote.Repositories
{
    public class SqlTranslationsRepository : SqlRepository<SqlTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public SqlTranslationsRepository(DbContextOptions<SqlContext> dbConfig, FileService fileService, string _userId) : base(dbConfig, fileService, _userId) { }
        protected override RecordType RecordType => RecordType.Translation;
        protected override TranslationModel FromEntityShortcut(SqlTranslation translation)
        {
            return TranslationModel.FromEntity(translation);
        }
        public override async Task<List<TranslationModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();
            var ids = await _dbContext.Set<SqlTranslation>().Select(e => e.TranslationId).ToListAsync();
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            var entities = await _dbContext.Set<SqlTranslation>()
              .Where(e => randomlySelectedIds.Contains(e.TranslationId))
              .AsNoTracking()
              .ToListAsync();

            var models = entities.Select(FromEntityShortcut).ToList();
            return models;
        }
        public override async Task<List<ChangeSetModel>> Add(TranslationDto dto)
        {
            // Set rate
            var user = UserModel.FromEntity(_dbContext.Users.Find(_userId));
            dto.Rate = user.GetRateRange().Lower;

            var entity = SqlTranslation.FromDto(dto, _dbContext);
            _dbContext.Translations.Add(entity);

            var changeSet = CreateChangeSetEntity(Operation.Insert, dto.TranslationId);
            _dbContext.ChangeSets.Add(changeSet);

            await _dbContext.SaveChangesAsync();

            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> Update(TranslationDto dto)
        {
            // Find out what has been changed
            var existing = await GetAsync(dto.TranslationId);
            var existingDto = TranslationDto.FromModel(existing);

            // This should be before checking for privileges, in case when input is existing entry and nothing has been changed
            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                return new List<ChangeSetModel>();
            }

            var user = UserModel.FromEntity(_dbContext.Users.Find(_userId));
            if (!user.CanEdit(existing.Rate, existing.UserId))
            {
                throw new InvalidOperationException("Error:Insufficient_privileges");
            }
          
            // Set rate, save
            changes.Add(new Change() { Property = "Rate", OldValue = dto.Rate, NewValue = user.GetRateRange().Lower });
            ApplyChanges<SqlTranslation>(dto.TranslationId, changes);

            var changeSet = CreateChangeSetEntity(Operation.Update, dto.TranslationId, changes);
            _dbContext.ChangeSets.Add(changeSet);

            await _dbContext.SaveChangesAsync();

            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> Remove(string entityId)
        {
            var existing = await GetAsync(entityId);
            var user = UserModel.FromEntity(_dbContext.Users.Find(_userId));
            if (!user.CanEdit(existing.Rate, existing.UserId))
            {
                throw new InvalidOperationException("Error:Insufficient_privileges");
            }

            return await base.Remove(entityId);
        }
        public async Task<ChangeSetModel> Promote(ITranslation translationInfo)
        {
            var translationEntity = await _dbContext.Translations.FindAsync(translationInfo.TranslationId);
            if (translationEntity == null)
            {
                throw new NullReferenceException();
            }

            var userEntity = await _dbContext.Users.FindAsync(_userId);
            var user = UserModel.FromEntity(userEntity);

            var newRate = user.GetRateRange().Lower;

            translationEntity.Rate = newRate;
            await _dbContext.SaveChangesAsync();

            var changes = new List<Change>() { new Change() { OldValue = translationEntity.Rate, NewValue = newRate, Property = "Rate" } };
            var changeSet = CreateChangeSetEntity(Operation.Update, translationEntity.TranslationId, changes);
            _dbContext.ChangeSets.Add(changeSet);
            await _dbContext.SaveChangesAsync();

            return ChangeSetModel.FromEntity(changeSet);
        }
    }
}
