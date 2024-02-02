using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.realm.RealmEntities;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.Interfaces;

namespace chldr_data.Repositories
{
    public class RealmTranslationsRepository : RealmRepository<RealmTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public RealmTranslationsRepository(IExceptionHandler exceptionHandler, IFileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }

        protected override RecordType RecordType => RecordType.Translation;
        public override TranslationModel FromEntity(RealmTranslation entity)
        {
            return TranslationModel.FromEntity(entity);
        }
        public override async Task<List<ChangeSetModel>> Add(TranslationDto dto)
        {
            _dbContext.Write(() =>
            {
                var translation = RealmTranslation.FromDto(dto, _dbContext);
                _dbContext.Add(translation);
            });

            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }
        public override async Task<List<ChangeSetModel>> Update(TranslationDto dto)
        {
            var existingEntity = await GetAsync(dto.TranslationId);
            var existingDto = TranslationDto.FromModel(existingEntity);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                // ! NOT IMPLEMENTED
                return new List<ChangeSetModel>();
            }

            _dbContext.Write(() =>
            {
                var updatedEntry = RealmTranslation.FromDto(dto, _dbContext);
            });
            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }

        public override Task<List<ChangeSetModel>> Remove(string entityId)
        {
            throw new NotImplementedException();
        }

        public Task<ChangeSetModel> Promote(ITranslation translation)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(string[] translations)
        {
            throw new NotImplementedException();
        }
    }
}
