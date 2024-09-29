using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Models;
using Microsoft.EntityFrameworkCore;
using domain.Interfaces.Repositories;
using sql_dl.SqlEntities;
using domain.Models;
using domain.DatabaseObjects.Interfaces;
using domain.Interfaces;
using domain;
using domain.Enums;

namespace sql_dl.Repositories
{
    public class SqlTranslationsRepository : SqlRepository<SqlTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {    
        public SqlTranslationsRepository(SqlContext context, IFileService fileService, string _userId) : base(context, fileService, _userId) { }
        protected override RecordType RecordType => RecordType.Translation;
     
        public override async Task<List<TranslationModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();
            var ids = await _dbContext.Set<SqlTranslation>().Select(e => e.TranslationId).ToListAsync();
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            var entities = await _dbContext.Set<SqlTranslation>()
              .Where(e => randomlySelectedIds.Contains(e.TranslationId))
              .AsNoTracking()
              .ToListAsync();

            var models = entities.Select(TranslationModel.FromEntity).ToList();
            return models;
        }
        public override async Task<List<ChangeSetModel>> AddAsync(TranslationDto dto)
        {
            // Set rate
            var user = UserModel.FromEntity(_dbContext.Users.Find(_userId) as SqlUser);
            dto.Rate = user.GetRateRange().Lower;

            var entity = SqlTranslation.FromDto(dto, _dbContext);
            _dbContext.Translations.Add(entity);

            var changeSet = CreateChangeSetEntity(Operation.Insert, dto.TranslationId);
            _dbContext.ChangeSets.Add(changeSet);

            await _dbContext.SaveChangesAsync();

            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> UpdateAsync(TranslationDto dto)
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

            var user = UserModel.FromEntity(_dbContext.Users.Find(_userId) as SqlUser);
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

        public override async Task<List<ChangeSetModel>> RemoveAsync(string entityId)
        {
            var existing = await GetAsync(entityId);
            var user = UserModel.FromEntity(_dbContext.Users.Find(_userId) as SqlUser);
            if (!user.CanEdit(existing.Rate, existing.UserId))
            {
                throw new InvalidOperationException("Error:Insufficient_privileges");
            }

            return await base.RemoveAsync(entityId);
        }
        public async Task<ChangeSetModel> Promote(ITranslation translationInfo)
        {
            var translationEntity = await _dbContext.Translations.FindAsync(translationInfo.TranslationId);
            if (translationEntity == null)
            {
                throw new NullReferenceException();
            }

            var userEntity = await _dbContext.Users.FindAsync(_userId);
            var user = UserModel.FromEntity(userEntity as SqlUser);

            var newRate = user.GetRateRange().Lower;

            translationEntity.Rate = newRate;
            await _dbContext.SaveChangesAsync();

            var changes = new List<Change>() { new Change() { OldValue = translationEntity.Rate, NewValue = newRate, Property = "Rate" } };
            var changeSet = CreateChangeSetEntity(Operation.Update, translationEntity.TranslationId, changes);
            _dbContext.ChangeSets.Add(changeSet);
            await _dbContext.SaveChangesAsync();

            return ChangeSetModel.FromEntity(changeSet);
        }

        public override async Task<TranslationModel> GetAsync(string entityId)
        {
            var translation = await _dbContext.Translations.FirstOrDefaultAsync(t => t.TranslationId!.Equals(entityId));
            if (translation == null)
            {
                throw new NullReferenceException();
            }

            return TranslationModel.FromEntity(translation);
        }

        public override async Task<List<TranslationModel>> TakeAsync(int offset, int limit)
        {
            var entities = await _dbContext.Translations
             .Skip(offset)
             .Take(limit)
             .ToListAsync();

            return entities.Select(TranslationModel.FromEntity).ToList();
        }

        public Task RemoveRange(string[] translations)
        {
            throw new NotImplementedException();
        }
    }
}
