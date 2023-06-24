using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.local.RealmEntities;
using Realms;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.Models;
using System.Threading.Channels;

namespace chldr_data.Repositories
{
    public class RealmTranslationsRepository : RealmRepository<RealmTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public RealmTranslationsRepository(Realm context, ExceptionHandler exceptionHandler, FileService fileService) : base(context, exceptionHandler, fileService) { }

        protected override RecordType RecordType => RecordType.Translation;
        protected override TranslationModel FromEntityShortcut(RealmTranslation entity)
        {
            return TranslationModel.FromEntity(entity);
        }
        public override void Add(TranslationDto dto)
        {
            _dbContext.Write(() =>
            {
                var translation = RealmTranslation.FromDto(dto, _dbContext);
                _dbContext.Add(translation);
            });

            InsertChangeSet(Operation.Insert, dto.UserId!, dto.TranslationId);
        }
        public override void Update(TranslationDto dto)
        {
            var existingEntity = Get(dto.TranslationId);
            var existingDto = TranslationDto.FromModel(existingEntity);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                return;
            }

            _dbContext.Write(() =>
            {
                var updatedEntry = RealmTranslation.FromDto(dto, _dbContext);
            });

            InsertChangeSet(Operation.Update, dto.UserId!, dto.TranslationId, changes);
        }
    }
}
