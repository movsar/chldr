using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;

namespace chldr_data.remote.Repositories
{
    internal abstract class SqlEntriesRepository<TModel, TDto> : SqlRepository<TModel, TDto>
        where TDto : class, new()
        where TModel : class
    {

        public event Action<EntryModel>? EntryUpdated;
        public event Action<EntryModel>? EntryInserted;
        public event Action<EntryModel>? EntryDeleted;
        public event Action<EntryModel>? EntryAdded;

        protected SqlEntriesRepository(SqlContext context, string userId) : base(context, userId)
        { }

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
        protected override abstract RecordType RecordType { get; }

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
