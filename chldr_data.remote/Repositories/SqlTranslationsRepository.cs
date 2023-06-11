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
    internal class SqlTranslationsRepository : SqlRepository<TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public SqlTranslationsRepository(SqlContext context, string _userId) : base(context, _userId) { }
        protected override RecordType RecordType => RecordType.Translation;

        public override void Delete(string entityId)
        {
            throw new NotImplementedException();
        }

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
        public override void Insert(TranslationDto dto)
        {
            var entity = SqlTranslation.FromDto(dto, _dbContext);
            _dbContext.Add(entity);

            InsertChangeSet(Operation.Insert, _userId, dto.TranslationId);
        }

        public override void Update(TranslationDto dto)
        {
            // Find out what has been changed
            var existing = Get(dto.TranslationId);
            var existingDto = TranslationDto.FromModel(existing);
            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                return;
            }

            ApplyChanges<SqlTranslation>(dto.TranslationId, changes);

            InsertChangeSet(Operation.Update, _userId, dto.TranslationId, changes);
        }
    }
}
