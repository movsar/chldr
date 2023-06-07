using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.DatabaseObjects.Models;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using chldr_data.Enums;
using chldr_data.Services;

namespace chldr_data.Repositories
{
    public class RealmTranslationsRepository : RealmRepository<SqlTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public RealmTranslationsRepository(SqlContext context) : base(context) { }

        protected override RecordType RecordType => RecordType.Translation;

        public override IEnumerable<ChangeSetModel> Add(string userId, TranslationDto dto)
        {
            var entity = SqlTranslation.FromDto(dto);
            _context.Add(entity);

            // Insert changeset
            var changeSet = new SqlChangeSet()
            {
                UserId = userId!,
                Operation = (int)Operation.Insert,
                RecordId = dto.TranslationId!,
                RecordType = (int)RecordType,
            };
            _context.ChangeSets.Add(changeSet);
            _context.SaveChanges();

            // Return changeset with updated index
            var changeSetModel = ChangeSetModel.FromEntity(_context.ChangeSets.Find(changeSet.ChangeSetId)!);
            return new List<ChangeSetModel>() { changeSetModel };
        }


        public override TranslationModel Get(string entityId)
        {
            var translation = _context.Translations
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
            var changes = SqlUnitOfWork.GetChanges(translationDto, existingDto);
            if (changes.Count == 0)
            {
                return EmptyResult;
            }

            ApplyChanges(translationDto.TranslationId, changes);

            // Insert changeset
            var changeSet = new SqlChangeSet()
            {
                UserId = userId!,
                Operation = (int)Operation.Update,
                RecordId = translationDto.TranslationId!,
                RecordType = (int)RecordType,
                RecordChanges = JsonConvert.SerializeObject(changes)
            };
            _context.ChangeSets.Add(changeSet);
            _context.SaveChanges();

            // Return changeset with updated index
            var changeSetModel = ChangeSetModel.FromEntity(_context.ChangeSets.Find(changeSet.ChangeSetId)!);
            return new List<ChangeSetModel>() { changeSetModel };
        }
    }
}
