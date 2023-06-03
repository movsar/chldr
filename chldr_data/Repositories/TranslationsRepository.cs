using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.RealmEntities;
using MongoDB.Bson;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.DatabaseObjects.Models;
using chldr_tools;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models.Words;
using Microsoft.EntityFrameworkCore;
using Realms.Sync;
using Realms;
using Newtonsoft.Json;
using chldr_data.Enums;

namespace chldr_data.Repositories
{
    public class TranslationsRepository : Repository<SqlTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public TranslationsRepository(SqlContext context) : base(context) { }

        protected override RecordType RecordType { get; set; } = RecordType.Translation;

        public override IEnumerable<ChangeSetModel> Add(string userId, TranslationDto dto)
        {
            var entity = SqlTranslation.FromDto(dto);
            SqlContext.Add(entity);

            // Insert changeset
            var changeSetDto = new ChangeSetDto()
            {
                UserId = userId!,
                Operation = Operation.Insert,
                RecordId = dto.TranslationId!,
                RecordType = RecordType,
            };
            using var unitOfWork = new UnitOfWork(SqlContext);
            unitOfWork.ChangeSets.Add(userId, changeSetDto);

            // Return changeset with updated index
            var changeSetModel = unitOfWork.ChangeSets.Get(changeSetDto.ChangeSetId);
            return new List<ChangeSetModel>() { changeSetModel };
        }

    
        public override TranslationModel Get(string entityId)
        {
            var translation = SqlContext.Translations
                .Include(translation => translation.Language)
                .FirstOrDefault(t => t.TranslationId.Equals(entityId));

            if (translation == null)
            {
                throw new ArgumentException($"Entity not found: {entityId}");
            }

            return TranslationModel.FromEntity(translation, translation.Language);
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, TranslationDto translationDto)
        {
            // Find out what has been changed
            var existing = Get(translationDto.TranslationId);
            var existingDto = TranslationDto.FromModel(existing);
            var changes = UnitOfWork.GetChanges(translationDto, existingDto);
            if (changes.Count == 0)
            {
                return EmptyResult;
            }

            ApplyChanges(translationDto.TranslationId, changes);

            // Insert changeset
            var changeSetDto = new ChangeSetDto()
            {
                UserId = userId!,
                Operation = Enums.Operation.Update,
                RecordId = translationDto.TranslationId!,
                RecordType = Enums.RecordType.Translation,
                RecordChanges = JsonConvert.SerializeObject(changes)
            };
            using var unitOfWork = new UnitOfWork(SqlContext);
            unitOfWork.ChangeSets.Add(userId, changeSetDto);

            // Return changeset with updated index
            var changeSetModel = unitOfWork.ChangeSets.Get(changeSetDto.ChangeSetId);
            return new List<ChangeSetModel>() { changeSetModel };
        }
    }
}
