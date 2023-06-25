using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Realms.Sync;
using Realms;
using System.Linq;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("chldr_data.remote.tests")]

namespace chldr_data.remote.Repositories
{
    internal class SqlEntriesRepository : SqlRepository<SqlEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        private readonly ExceptionHandler _exceptionHandler;
        private readonly SqlTranslationsRepository _translations;
        private readonly SqlSoundsRepository _sounds;

        public SqlEntriesRepository(
            SqlContext context,
            FileService fileService,
            ExceptionHandler exceptionHandler,
            ITranslationsRepository translationsRepository,
            ISoundsRepository soundsRepository,
            string userId) : base(context, fileService, userId)
        {
            _exceptionHandler = exceptionHandler;
            _translations = (SqlTranslationsRepository)translationsRepository;
            _sounds = (SqlSoundsRepository)soundsRepository;
        }

        protected override RecordType RecordType => RecordType.Entry;

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
        public override async Task<IEnumerable<EntryModel>> TakeAsync(int offset, int limit)
        {
            var entities = await _dbContext.Set<SqlEntry>()
                      .Include(e => e.Source)
                      .Include(e => e.User)
                      .Include(e => e.Translations)
                      .Include(e => e.Sounds)

                      .Skip(offset)
                      .Take(limit)

                      .ToListAsync();

            return entities.Select(FromEntityShortcut).ToList();
        }
        public override async Task<List<EntryModel>> GetRandoms(int limit)
        {
            var randomizer = new Random();

            var entries = await _dbContext.Set<SqlEntry>()
                .Include(e => e.Source)
                .Include(e => e.User)
                .Include(e => e.Translations)
                .Include(e => e.Sounds)
                .OrderBy(x => randomizer.Next(0, 75000))
                .OrderBy(entry => entry.GetHashCode())
                .Take(limit)
                .Select(entry => FromEntityShortcut(entry))

                .ToListAsync();

            return entries;
        }
        protected override EntryModel FromEntityShortcut(SqlEntry entry)
        {
            return EntryModel.FromEntity(entry, entry.Source, entry.Translations, entry.Sounds);
        }

        public override List<ChangeSetModel> Add(EntryDto newEntryDto)
        {
            if (newEntryDto == null || string.IsNullOrEmpty(newEntryDto.EntryId))
            {
                throw new NullReferenceException();
            }

            var changeSets = new List<ChangeSetModel>();

            try
            {
                // Insert Entry entity (with associated sound and translation entities)
                var entry = SqlEntry.FromDto(newEntryDto, _dbContext);
                _dbContext.Add(entry);
                _dbContext.SaveChanges();

                // Set CreatedAt to update it on local entry
                newEntryDto.CreatedAt = entry.CreatedAt;

                var entryChangeSet = CreateChangeSetEntity(Operation.Insert, newEntryDto.EntryId);
                changeSets.Add(ChangeSetModel.FromEntity(entryChangeSet));

                // Process added translations
                foreach (var translation in newEntryDto.Translations)
                {
                    var translationChangeSets = _translations.Add(translation);
                    changeSets.AddRange(translationChangeSets);
                }

                // Process added sounds
                foreach (var sound in newEntryDto.Sounds)
                {
                    if (string.IsNullOrEmpty(sound.RecordingB64))
                    {
                        continue;
                    }

                    var filePath = Path.Combine(_fileService.EntrySoundsDirectory, sound.FileName);
                    File.WriteAllText(filePath, sound.RecordingB64);
                    _sounds.Add(sound);

                    var soundChangeSets = _sounds.Add(sound);
                    changeSets.AddRange(soundChangeSets);
                }

                return changeSets;
            }
            catch (Exception ex)
            {
                throw _exceptionHandler.Error(ex);
            }
        }
        public override List<ChangeSetModel> Update(EntryDto updatedEntryDto)
        {
            var changeSets = new List<ChangeSetModel>();

            try
            {
                var existingEntry = Get(updatedEntryDto.EntryId);
                var existingEntryDto = EntryDto.FromModel(existingEntry);

                // Update entry
                var entryChanges = Change.GetChanges(updatedEntryDto, existingEntryDto);
                if (entryChanges.Count != 0)
                {
                    // Save the changes, even if there are no changes to the entry, as there might be 
                    var updatedEntryEntity = SqlEntry.FromDto(updatedEntryDto, _dbContext);
                    _dbContext.Update(updatedEntryEntity);
                    _dbContext.SaveChanges();

                    var entryChangeSetDto = CreateChangeSetEntity(Operation.Update, updatedEntryDto.EntryId, entryChanges);
                    changeSets.Add(ChangeSetModel.FromEntity(entryChangeSetDto));
                }

                // Add / Remove / Update translations
                var translationChangeSets = ProcessTranslationsForEntryUpdate(existingEntryDto, updatedEntryDto);
                changeSets.AddRange(translationChangeSets);

                // Add / Remove / Update sounds
                var soundChangeSets = ProcessSoundsForEntryUpdate(existingEntryDto, updatedEntryDto);
                changeSets.AddRange(soundChangeSets);

                // Insert changesets
                changeSets.AddRange(changeSets);
                return changeSets;
            }
            catch (Exception ex)
            {
                throw _exceptionHandler.Error(ex);
            }
        }
        public override List<ChangeSetModel> Remove(string entryId)
        {
            var changeSets = new List<ChangeSetModel>();

            try
            {
                var entry = Get(entryId);
                var soundIds = entry.Sounds.Select(s => s.SoundId).ToArray();
                var translationIds = entry.Translations.Select(t => t.TranslationId).ToArray();

                // Process removed translations
                foreach (var translationId in translationIds)
                {
                    var translationChangeSet = _translations.Remove(translationId);
                    changeSets.AddRange(translationChangeSet);
                }

                // Process removed sounds
                foreach (var soundId in soundIds)
                {

                    var translationChangeSet = _sounds.Remove(soundId);
                    changeSets.AddRange(translationChangeSet);
                }

                // Process removed entry
                var changeSet = base.Remove(entryId);
                changeSets.AddRange(changeSet);

                return changeSets;
            }
            catch (Exception ex)
            {
                throw _exceptionHandler.Error(ex);
            }
        }

        private List<ChangeSetModel> ProcessSoundsForEntryUpdate(EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            var changeSets = new List<ChangeSetModel>();

            var existingEntrySoundIds = existingEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();
            var updatedEntrySoundIds = updatedEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();

            var added = updatedEntryDto.Sounds.Where(t => !existingEntrySoundIds.Contains(t.SoundId));
            var deleted = existingEntryDto.Sounds.Where(t => !updatedEntrySoundIds.Contains(t.SoundId));
            var updated = updatedEntryDto.Sounds.Where(t => existingEntrySoundIds.Contains(t.SoundId) && updatedEntrySoundIds.Contains(t.SoundId));

            // Process inserted translations
            foreach (var sound in added)
            {
                var changeSet = _sounds.Add(sound);
                changeSets.AddRange(changeSet);
            }

            // Process removed translations
            foreach (var sound in deleted)
            {
                var changeSet = _sounds.Remove(sound.SoundId);
                changeSets.AddRange(changeSet);
            }

            // Process updated translations
            foreach (var sound in updated)
            {
                var existingDto = existingEntryDto.Sounds.First(t => t.SoundId.Equals(sound.SoundId));

                var changes = Change.GetChanges(sound, existingDto);
                if (changes.Count == 0)
                {
                    continue;
                }

                var changeSet = _sounds.Update(sound);
                changeSets.AddRange(changeSet);
            }

            return changeSets;
        }
        private List<ChangeSetModel> ProcessTranslationsForEntryUpdate(EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            var changeSets = new List<ChangeSetModel>();

            var existingEntryTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedEntryTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var added = updatedEntryDto.Translations.Where(t => !existingEntryTranslationIds.Contains(t.TranslationId));
            var deleted = existingEntryDto.Translations.Where(t => !updatedEntryTranslationIds.Contains(t.TranslationId));
            var updated = updatedEntryDto.Translations.Where(t => existingEntryTranslationIds.Contains(t.TranslationId) && updatedEntryTranslationIds.Contains(t.TranslationId));

            // Process inserted translations
            foreach (var translation in added)
            {
                var changeSet = _translations.Add(translation);
                changeSets.AddRange(changeSet);
            }

            // Process removed translations
            foreach (var translation in deleted)
            {
                var changeSet = _translations.Remove(translation.TranslationId);
                changeSets.AddRange(changeSet);
            }

            // Process updated translations
            foreach (var translation in updated)
            {
                var existingTranslationDto = existingEntryDto.Translations.First(t => t.TranslationId.Equals(translation.TranslationId));

                var changes = Change.GetChanges(translation, existingTranslationDto);
                if (changes.Count == 0)
                {
                    continue;
                }

                var changeSet = _translations.Update(translation);
                changeSets.AddRange(changeSet);
            }
            return changeSets;
        }

    }
}
