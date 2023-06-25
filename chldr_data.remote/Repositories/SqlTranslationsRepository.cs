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

namespace chldr_data.remote.Repositories
{
    internal class SqlTranslationsRepository : SqlRepository<SqlTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public SqlTranslationsRepository(SqlContext context, FileService fileService, string _userId) : base(context, fileService, _userId) { }
        protected override RecordType RecordType => RecordType.Translation;
        protected override TranslationModel FromEntityShortcut(SqlTranslation translation)
        {
            return TranslationModel.FromEntity(translation);
        }
     
        public override void Add(TranslationDto dto)
        {
            var entity = SqlTranslation.FromDto(dto, _dbContext);
            _dbContext.Add(entity);
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
        }

    }
}
