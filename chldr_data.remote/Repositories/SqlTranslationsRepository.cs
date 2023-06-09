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

namespace chldr_data.remote.Repositories
{
    public class SqlTranslationsRepository : SqlRepository<SqlTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public SqlTranslationsRepository(SqlContext context) : base(context) { }
        protected override RecordType RecordType => RecordType.Translation;

        public override TranslationModel Get(string entityId)
        {
            var translation = _dbContext.Translations
                .Include(translation => translation.Language)
                .FirstOrDefault(t => t.TranslationId.Equals(entityId));

            if (translation == null)
            {
                throw new ArgumentException($"Entity not found: {entityId}");
            }

            return TranslationModel.FromEntity(translation, translation.Language);
        }

        public override async Task Insert(string userId, TranslationDto dto)
        {
            var entity = SqlTranslation.FromDto(dto);
            _dbContext.Add(entity);

            InsertChangeSet(Operation.Insert, userId, dto.TranslationId);
        }

       
        public override async Task Update(string userId, TranslationDto translationDto)
        {
            // Find out what has been changed
            var existing = Get(translationDto.TranslationId);
            var existingDto = TranslationDto.FromModel(existing);
            var changes = Change.GetChanges(translationDto, existingDto);
            if (changes.Count == 0)
            {
                return ;
            }

            ApplyChanges<SqlTranslation>(translationDto.TranslationId, changes);
            InsertChangeSet(Operation.Update, userId, translationDto.TranslationId, changes);
        }

        internal Task Delete(string translationId)
        {
            throw new NotImplementedException();
        }

        internal Task Insert(TranslationDto translationDto)
        {
            throw new NotImplementedException();
        }

        internal Task Update(TranslationDto translationDto)
        {
            throw new NotImplementedException();
        }
    }
}
