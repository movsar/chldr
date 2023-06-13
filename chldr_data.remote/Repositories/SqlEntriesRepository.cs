using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.remote.Repositories
{
    internal class SqlEntriesRepository : SqlRepository<SqlEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        private readonly ITranslationsRepository _translations;

        public SqlEntriesRepository(SqlContext context, string userId, ITranslationsRepository translationsRepository) : base(context, userId)
        {
            _translations = translationsRepository;
        }

        protected override RecordType RecordType => RecordType.Entry;

        public event Action<EntryModel>? EntryUpdated;
        public event Action<EntryModel>? EntryInserted;
        public event Action<EntryModel>? EntryDeleted;
        public event Action<EntryModel>? EntryAdded;


        protected override EntryModel FromEntityShortcut(SqlEntry entry)
        {
            return EntryModel.FromEntity(
               entry,
               entry.Source,
               entry.Translations
           );
        }

   
        private SqlEntry GetEntity(string entityId)
        {
            var entry = _dbContext.Entries
                        .Include(w => w.User)
                        .Include(w => w.Source)
                        .Include(w => w.Translations)
                        .FirstOrDefault(w => w.EntryId.Equals(entityId));

            return entry;
        }

        public override void Add(EntryDto newEntryDto)
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

            // Handle associated translation changes
            var existingTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var addedTranslations = updatedEntryDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingEntryDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedEntryDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            _translations.AddRange(addedTranslations);
            _translations.RemoveRange(deletedTranslations.Select(t => t.TranslationId));
            _translations.UpdateRange(updatedTranslations);

            // Add changeset if applicable
            var entryChanges = Change.GetChanges(existingEntryDto, existingEntryDto);
            if (entryChanges.Count != 0)
            {
                InsertChangeSet(Operation.Update, _userId, existingEntryDto.EntryId, entryChanges);
            }

            // Save the changes
            var updatedEntryEntity = SqlEntry.FromDto(updatedEntryDto, _dbContext);
            _dbContext.Update(updatedEntryEntity);
            _dbContext.SaveChanges();
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
    }
}
