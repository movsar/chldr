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
            FileService fileService,
            ITranslationsRepository translationsRepository,
            ISoundsRepository soundsRepository,
            string userId) : base(context, fileService, userId)
        {
            _translations = translationsRepository;
            _sounds = soundsRepository;
        }

        protected override RecordType RecordType => RecordType.Entry;

        public override void Remove(string entityId)
        {
            var entry = _dbContext.Entries
                .Include(e => e.Translations)
                .Include(e => e.Sounds)
                .FirstOrDefault(e => e.EntryId.Equals(entityId));

            if (entry == null)
            {
                return;
            }

            var soundIds = entry.Sounds.Select(s => s.SoundId).ToArray();
            var translationIds = entry.Translations.Select(t => t.TranslationId).ToArray();

            _translations.RemoveRange(translationIds);
            _sounds.RemoveRange(soundIds);

            base.Remove(entityId);
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

        public override void Add(EntryDto newEntryDto)
        {
            if (newEntryDto == null || string.IsNullOrEmpty(newEntryDto.EntryId))
            {
                throw new NullReferenceException();
            }

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

                var filePath = Path.Combine(_fileService.EntrySoundsDirectory, soundDto.FileName);
                File.WriteAllText(filePath, soundDto.RecordingB64);
            }

            // Set CreatedAt to update it on local entry
            newEntryDto.CreatedAt = entry.CreatedAt;
        }

        public override void Update(EntryDto updatedEntryDto)
        {
            var existingEntry = Get(updatedEntryDto.EntryId);
            var existingEntryDto = EntryDto.FromModel(existingEntry);

            // Add changeset if applicable
            var entryChanges = Change.GetChanges(updatedEntryDto, existingEntryDto);
            if (entryChanges.Count == 0)
            {
                return;
            }

            // Save the changes, even if there are no changes to the entry, as there might be 
            var updatedEntryEntity = SqlEntry.FromDto(updatedEntryDto, _dbContext);
            _dbContext.Update(updatedEntryEntity);
            _dbContext.SaveChanges();
        }
    }
}
