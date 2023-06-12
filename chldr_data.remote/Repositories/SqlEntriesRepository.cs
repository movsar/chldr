using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace chldr_data.remote.Repositories
{
    internal class SqlEntriesRepository : SqlRepository<EntryModel, EntryDto>, IEntriesRepository
    {
        public SqlEntriesRepository(SqlContext context, string userId) : base(context, userId)
        {
        }

        protected override RecordType RecordType => RecordType.Entry;

        public event Action<EntryModel>? EntryUpdated;
        public event Action<EntryModel>? EntryInserted;
        public event Action<EntryModel>? EntryDeleted;
        public event Action<EntryModel>? EntryAdded;


        public static EntryModel FromEntity(SqlEntry entry)
        {
            return EntryModel.FromEntity(
                entry,
                entry.Source,
                entry.Translations.Select(t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t))
            );
        }

        private SqlEntry GetEntity(string entityId)
        {
            var entry = _dbContext.Entries
                        .Include(w => w.User)
                        .Include(w => w.Source)
                        .Include(w => w.Translations)
                        .ThenInclude(t => t.Language)
                        .FirstOrDefault(w => w.EntryId.Equals(entityId));

            return entry;
        }
        public override EntryModel Get(string entityId)
        {
            var entry = GetEntity(entityId);
            if (entry == null)
            {
                throw new ArgumentException($"Entity not found: {entityId}");
            }

            return FromEntity(entry);
        }

        public override void Delete(string entityId)
        {
            throw new NotImplementedException();
        }

        public override void Insert(EntryDto newEntryDto)
        {
            if (!newEntryDto.Translations.Any())
            {
                throw new Exception("Empty translations");
            }

            // Insert Entry entity
            var entry = SqlEntry.FromDto(newEntryDto, _dbContext);
            _dbContext.Add(entry);
            _dbContext.SaveChanges();

            // Set CreatedAt to update it on local entry
            newEntryDto.CreatedAt = entry.CreatedAt;

            // Insert a change set
            InsertChangeSet(Operation.Insert, _userId, newEntryDto.EntryId);
            // Insert new translation changesets? - not necessary, but could be used for audit
        }

        public override void Update(EntryDto updatedEntryDto)
        {
            var existingEntry = Get(updatedEntryDto.EntryId);
            var existingEntryDto = EntryDto.FromModel(existingEntry);
           
            var updatedEntryEntity = SqlEntry.FromDto(updatedEntryDto, _dbContext);
            var existingEntryEntity = GetEntity(updatedEntryDto.EntryId);

            // Remove translations that are no longer associated with updatedEntryEntity
            var removedTranslations = existingEntryEntity.Translations.Except(updatedEntryEntity.Translations);
            foreach (var translation in removedTranslations)
            {
                _dbContext.Translations.Remove(translation);
            }

            _dbContext.Update(updatedEntryEntity);
            _dbContext.SaveChanges();

            InsertEntryUpdateChangeSets(existingEntryDto, updatedEntryDto);
        }


        protected void InsertEntryUpdateChangeSets(EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            var entryChanges = Change.GetChanges(existingEntryDto, existingEntryDto);
            if (entryChanges.Count != 0)
            {
                InsertChangeSet(Operation.Update, _userId, existingEntryDto.EntryId, entryChanges);
            }

            var existingTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var insertedTranslations = updatedEntryDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingEntryDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedEntryDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            foreach (var translation in insertedTranslations)
            {
                InsertChangeSet(Operation.Insert, _userId, translation.EntryId);
            }

            foreach (var translation in deletedTranslations)
            {
                InsertChangeSet(Operation.Delete, _userId, translation.EntryId);
            }

            foreach (var translation in updatedTranslations)
            {
                var changes = Change.GetChanges(translation, existingEntryDto.Translations.First(t => t.TranslationId.Equals(translation.TranslationId)));
                InsertChangeSet(Operation.Update, _userId, translation.TranslationId, changes);
            }
        }
        public async Task<bool> CanCreateOrUpdateEntry(SqlEntry entry)
        {
            if (entry.ParentEntryId == entry.EntryId)
            {
                // Self-linking is not allowed
                return false;
            }

            var isCircularReference = await IsCircularReference(entry.EntryId, entry.ParentEntryId);
            if (isCircularReference)
            {
                // Circular linking is not allowed
                return false;
            }

            return true;
        }

        private async Task<bool> IsCircularReference(string entryId, string parentEntryId)
        {
            var currentEntry = await _dbContext.Entries.FindAsync(entryId);
            while (currentEntry != null)
            {
                if (currentEntry.EntryId == parentEntryId)
                {
                    // Circular reference detected
                    return true;
                }

                //currentEntry = currentEntry.ParentEntry;
            }

            return false;
        }

        public EntryModel GetByEntryId(string entryId)
        {
            throw new NotImplementedException();
        }
    }
}
