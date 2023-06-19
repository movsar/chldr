using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace chldr_data.remote.Repositories
{
    internal class SqlEntriesRepository : SqlRepository<SqlEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        private readonly ITranslationsRepository _translations;
        private readonly ISoundsRepository _sounds;

        public SqlEntriesRepository(
            SqlContext context,
            ITranslationsRepository translationsRepository,
            ISoundsRepository soundsRepository,
            string userId) : base(context, userId)
        {
            _translations = translationsRepository;
            _sounds = soundsRepository;
        }

        protected override RecordType RecordType => RecordType.Entry;

        public override void Remove(string entityId)
        {
            var entity = _dbContext.Entries.Include(e => e.Translations).FirstOrDefault(e => e.EntryId.Equals(entityId));
            if (entity == null)
            {
                throw new ArgumentException("Entity doesn't exist");
            }

            foreach (var translation in entity.Translations)
            {
                _dbContext.Remove(translation);
            }
            _dbContext.Remove(entity);

            InsertChangeSet(Operation.Delete, _userId, entityId);
        }
        public override EntryModel Get(string entityId)
        {
            var entry = _dbContext.Entries
                .Include(e => e.Source)
                .Include(e => e.User)
                .Include(e => e.Translations)
                .Include(e => e.Sounds)
                .FirstOrDefault(e => e.EntryId.Equals(entityId));

            if (entry == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntityShortcut(entry);
        }
        public override IEnumerable<EntryModel> Take(int limit)
        {
            var entities = _dbContext.Set<SqlEntry>()
                .Include(e => e.Source)
                .Include(e => e.User)
                .Include(e => e.Translations)
                .Include(e => e.Sounds)
                .Take(limit);

            return entities.Select(e => FromEntityShortcut(e));
        }
        public override IEnumerable<EntryModel> GetRandoms(int limit)
        {
            var randomizer = new Random();

            var entries = _dbContext.Set<SqlEntry>()
                .Include(e => e.Source)
                .Include(e => e.User)
                .Include(e => e.Translations)
                .Include(e => e.Sounds)
                .OrderBy(x => randomizer.Next(0, 75000))
                .OrderBy(entry => entry.GetHashCode())
                .Take(limit)
                .Select(entry => FromEntityShortcut(entry));

            return entries;
        }
        protected override EntryModel FromEntityShortcut(SqlEntry entry)
        {
            return EntryModel.FromEntity(
               entry,
               entry.Source,
               entry.Translations,
               entry.Sounds
           );
        }

        public override void Add(EntryDto newEntryDto)
        {
            // Insert Entry entity (with associated sound and translation entities)
            var entry = SqlEntry.FromDto(newEntryDto, _dbContext);
            _dbContext.Add(entry);
            _dbContext.SaveChanges();

            // Save audiofiles if any
            foreach (var soundDto in newEntryDto.Sounds)
            {
                if (string.IsNullOrEmpty(soundDto.RecordingB64))
                {
                    continue;
                }

                var filePath = Path.Combine(FileService.EntrySoundsDirectory, soundDto.FileName);
                File.WriteAllText(filePath, soundDto.RecordingB64);
            }

            // Set CreatedAt to update it on local entry
            newEntryDto.CreatedAt = entry.CreatedAt;

            // Insert a change set
            InsertChangeSet(Operation.Insert, _userId, newEntryDto.EntryId);

            // ! Insert new translation changesets? - not necessary, but could be used for audit
        }

        private void HandleUpdatedEntryTranslations(EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            // Handle associated translation changes
            var existingEntryTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedEntryTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var added = updatedEntryDto.Translations.Where(t => !existingEntryTranslationIds.Contains(t.TranslationId));
            var deleted = existingEntryDto.Translations.Where(t => !updatedEntryTranslationIds.Contains(t.TranslationId));
            var updated = updatedEntryDto.Translations.Where(t => existingEntryTranslationIds.Contains(t.TranslationId) && updatedEntryTranslationIds.Contains(t.TranslationId));

            _translations.AddRange(added);
            _translations.RemoveRange(deleted.Select(t => t.TranslationId));
            _translations.UpdateRange(updated);
        }

        private void HandleUpdatedEntrySounds(EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            // Handle associated translation changes
            var existingEntrySoundIds = existingEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();
            var updatedEntrySoundIds = updatedEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();

            var added = updatedEntryDto.Sounds.Where(t => !existingEntrySoundIds.Contains(t.SoundId));
            var deleted = existingEntryDto.Sounds.Where(t => !updatedEntrySoundIds.Contains(t.SoundId));
            var updated = updatedEntryDto.Sounds.Where(t => existingEntrySoundIds.Contains(t.SoundId) && updatedEntrySoundIds.Contains(t.SoundId));

            _sounds.AddRange(added);
            _sounds.RemoveRange(deleted.Select(t => t.SoundId));
            _sounds.UpdateRange(updated);
        }

        public override void Update(EntryDto updatedEntryDto)
        {
            var existingEntry = Get(updatedEntryDto.EntryId);
            var existingEntryDto = EntryDto.FromModel(existingEntry);

            HandleUpdatedEntryTranslations(existingEntryDto, updatedEntryDto);

            HandleUpdatedEntrySounds(existingEntryDto, updatedEntryDto);

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
