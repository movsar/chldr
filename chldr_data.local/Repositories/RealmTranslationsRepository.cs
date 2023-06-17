using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.local.RealmEntities;
using Realms;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;

namespace chldr_data.Repositories
{
    public class RealmTranslationsRepository : RealmRepository<RealmTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public RealmTranslationsRepository(Realm context, ExceptionHandler exceptionHandler) : base(context, exceptionHandler) { }
        
        protected override RecordType RecordType => RecordType.Translation;
        protected override TranslationModel FromEntityShortcut(RealmTranslation entity)
        {
            return TranslationModel.FromEntity(entity);
        }

        public override void Update(TranslationDto translationDto)
        {
            _dbContext.Write(() =>
            {
                var updatedEntry = RealmTranslation.FromDto(translationDto, _dbContext);
            });
        }
        public override void Add(TranslationDto translationDto)
        {
            _dbContext.Write(() =>
            {
                var translation = RealmTranslation.FromDto(translationDto, _dbContext);
                _dbContext.Add(translation);
            });
        }
    }
}
