using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using chldr_utils.Models;
using chldr_data.Services;
using Realms.Sync;
using chldr_data.DatabaseObjects.Interfaces;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("chldr_data.remote.tests")]

namespace chldr_data.remote.Repositories
{
    public class SqlEntriesRepository : SqlRepository<SqlEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        private readonly ExceptionHandler _exceptionHandler;
        private readonly SqlTranslationsRepository _translations;
        private readonly SqlSoundsRepository _sounds;

        public event Action<SearchResultModel>? GotNewSearchResult;

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

        public override async Task<EntryModel> Get(string entityId)
        {
            var entry = await _dbContext.Entries
                .Include(e => e.Source)
                .Include(e => e.User)
                .Include(e => e.Translations)
                .Include(e => e.Sounds)
                .FirstOrDefaultAsync(e => e.EntryId.Equals(entityId));

            if (entry == null)
            {
                throw new ArgumentException("There is no such word in the database");
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
        public override async Task<List<EntryModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();

            // Fetch all EntryIds from the database
            var ids = await _dbContext.Set<SqlEntry>().Where(e => e.ParentEntryId == null).Select(e => e.EntryId).ToListAsync();

            // Shuffle the ids and take what's needed
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            // Fetch entries with the selected EntryIds
            var entities = await _dbContext.Set<SqlEntry>()
                .Where(entry => randomlySelectedIds.Contains(entry.EntryId))
                .Include(e => e.Source)
                .Include(e => e.User)
                .Include(e => e.Translations)
                .Include(e => e.Sounds)
                .AsNoTracking()
                .ToListAsync();

            // Apply the projection (mapping to FromEntityShortcut) on the in-memory data
            var models = entities.Select(FromEntityShortcut).ToList();
            return models;
        }
        protected override EntryModel FromEntityShortcut(SqlEntry entry)
        {
            return EntryModel.FromEntity(entry, entry.Source, entry.Translations, entry.Sounds);
        }

        public async Task<List<EntryModel>> FindAsync(string inputText, FiltrationFlags filtrationFlags)
        {
            var result = new List<EntryModel>();
            IQueryable<SqlEntry> query;

            if (string.IsNullOrEmpty(inputText) || inputText.Length <= 3)
            {
                query = _dbContext.Entries
                    .Where(e => e.RawContents.Equals(inputText, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                query = _dbContext.Entries
                    .Where(e => e.RawContents.StartsWith(inputText, StringComparison.OrdinalIgnoreCase));
            }

            // Get matching entry IDs
            var matchingEntryIds = await query
                .Select(e => e.EntryId)
                .ToListAsync();

            // Apply the shortcut method to the matching entry IDs            
            result = matchingEntryIds
                .Select(id => FromEntityShortcut(_dbContext.Entries
                    .Include(e => e.Source)
                    .Include(e => e.User)
                    .Include(e => e.Translations)
                    .Include(e => e.Sounds)
                    .AsNoTracking()
                    .First(e => e.EntryId.Equals(id))))
                .ToList();

            // Sort
            SearchServiceHelper.SortDirectSearchEntries(inputText, result);

            return result;
        }

        public async Task FindDeferredAsync(string inputText, FiltrationFlags filtrationFlags)
        {
            var result = new List<EntryModel>();

            await Task.Run(async () =>
            {
                result = await FindAsync(inputText, filtrationFlags);
            });

            var args = new SearchResultModel(inputText, result, SearchResultModel.Mode.Full);
            GotNewSearchResult?.Invoke(args);
        }
        public List<EntryModel> GetLatestEntries()
        {
            var count = 50;

            var entries = _dbContext.Entries
                .Where(e => e.ParentEntryId == null)
                .OrderByDescending(e => e.CreatedAt)
                .Take(count)
                .Include(e => e.Source)
                .Include(e => e.User)
                .Include(e => e.Translations)
                .Include(e => e.Sounds)
                .AsNoTracking()
                .ToList();

            return entries.Select(entry => FromEntityShortcut(entry)).ToList();
        }

        public List<EntryModel> GetEntriesOnModeration()
        {
            var count = 50;

            var entries = _dbContext.Entries
                .Where(e => e.ParentEntryId == null)
                .Where(entry => entry.Rate < UserModel.MemberRateRange.Lower)
                .Take(count)
                .Include(e => e.Source)
                .Include(e => e.User)
                .Include(e => e.Translations)
                .Include(e => e.Sounds)
                .AsNoTracking()
                .ToList();

            return entries.Select(entry => FromEntityShortcut(entry)).ToList();
        }

        public override async Task<List<ChangeSetModel>> Add(EntryDto newEntryDto)
        {
            if (newEntryDto == null || string.IsNullOrEmpty(newEntryDto.EntryId))
            {
                throw new NullReferenceException();
            }

            var user = UserModel.FromEntity(await _dbContext.Users.FindAsync(_userId));
            if (user.Status != UserStatus.Active)
            {
                // The 199th line, the user.Status is always Active 
                throw new InvalidOperationException();
            }

            // Set rate
            newEntryDto.Rate = user.GetRateRange().Lower;
            foreach (var translationDto in newEntryDto.Translations)
            {
                translationDto.Rate = newEntryDto.Rate;
            }

            try
            {
                // Insert Entry entity (with associated sound and translation entities)
                var entry = SqlEntry.FromDto(newEntryDto, _dbContext);
                _dbContext.Add(entry);
                _dbContext.SaveChanges();

                // Set CreatedAt to update it on local entry
                newEntryDto.CreatedAt = entry.CreatedAt;

                var entryChangeSet = CreateChangeSetEntity(Operation.Insert, newEntryDto.EntryId);
                var resultingChangeSets = new List<ChangeSetModel>
                {
                    ChangeSetModel.FromEntity(entryChangeSet)
                };

                // Process added sounds
                foreach (var sound in newEntryDto.Sounds)
                {
                    if (string.IsNullOrEmpty(sound.RecordingB64))
                    {
                        continue;
                    }

                    var filePath = Path.Combine(_fileService.EntrySoundsDirectory, sound.FileName);
                    File.WriteAllText(filePath, sound.RecordingB64);
                }

                return resultingChangeSets;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.Message.Equals("Could not save changes. Please configure your entity type accordingly."))
                {
                    throw _exceptionHandler.Error(ex.InnerException);
                }
                throw _exceptionHandler.Error(ex);
            }
        }
        public override async Task<List<ChangeSetModel>> Update(EntryDto updatedEntryDto)
        {
            var resultingChangeSets = new List<ChangeSetModel>();

            try
            {
                var entry = await Get(updatedEntryDto.EntryId);
                var user = UserModel.FromEntity(await _dbContext.Users.FindAsync(_userId));
                var author = UserModel.FromEntity(await _dbContext.Users.FindAsync(entry.UserId));
                if (user.Status != UserStatus.Active || user.GetRateRange().Lower < author.GetRateRange().Lower)
                {
                    throw new InvalidOperationException();
                }

                updatedEntryDto.Rate = user.GetRateRange().Lower;
                foreach (var translationDto in updatedEntryDto.Translations)
                {
                    translationDto.Rate = updatedEntryDto.Rate;
                }

                var existingEntryDto = EntryDto.FromModel(entry);

                // Update entry
                var entryChanges = Change.GetChanges(updatedEntryDto, existingEntryDto);
                if (entryChanges.Count != 0)
                {
                    // Save the changes, even if there are no changes to the entry, as there might be 
                    var updatedEntryEntity = SqlEntry.FromDto(updatedEntryDto, _dbContext);
                    _dbContext.Update(updatedEntryEntity);

                    var entryChangeSetDto = CreateChangeSetEntity(Operation.Update, updatedEntryDto.EntryId, entryChanges);
                    resultingChangeSets.Add(ChangeSetModel.FromEntity(entryChangeSetDto));
                }

                // Add / Remove / Update translations
                var translationChangeSets = await ProcessTranslationsForEntryUpdate(existingEntryDto, updatedEntryDto);
                resultingChangeSets.AddRange(translationChangeSets);

                // Add / Remove / Update sounds
                var soundChangeSets = await ProcessSoundsForEntryUpdate(existingEntryDto, updatedEntryDto);
                resultingChangeSets.AddRange(soundChangeSets);

                _dbContext.SaveChanges();

                return resultingChangeSets;
            }
            catch (Exception ex)
            {
                throw _exceptionHandler.Error(ex);
            }
        }
        public override async Task<List<ChangeSetModel>> Remove(string entryId)
        {
            var changeSets = new List<ChangeSetModel>();

            try
            {
                var entry = await Get(entryId);
                var user = UserModel.FromEntity(await _dbContext.Users.FindAsync(_userId));
                var author = UserModel.FromEntity(await _dbContext.Users.FindAsync(entry.UserId));

                if (user.Status != UserStatus.Active || user.GetRateRange().Lower < author.GetRateRange().Lower)
                {
                    throw new InvalidOperationException();
                }

                var soundIds = entry.Sounds.Select(s => s.SoundId).ToArray();
                var translationIds = entry.Translations.Select(t => t.TranslationId).ToArray();

                // Process removed translations
                foreach (var translationId in translationIds)
                {
                    var translationChangeSet = await _translations.Remove(translationId);
                    changeSets.AddRange(translationChangeSet);
                }

                // Process removed sounds
                foreach (var soundId in soundIds)
                {

                    var translationChangeSet = await _sounds.Remove(soundId);
                    changeSets.AddRange(translationChangeSet);
                }

                // Process removed entry
                var changeSet = await base.Remove(entryId);
                changeSets.AddRange(changeSet);

                return changeSets;
            }
            catch (Exception ex)
            {
                throw _exceptionHandler.Error(ex);
            }
        }

        private async Task<List<ChangeSetModel>> ProcessSoundsForEntryUpdate(EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            var changeSets = new List<ChangeSetModel>();

            var existingEntrySoundIds = existingEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();
            var updatedEntrySoundIds = updatedEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();

            var added = updatedEntryDto.Sounds.Where(t => !existingEntrySoundIds.Contains(t.SoundId));
            var deleted = existingEntryDto.Sounds.Where(t => !updatedEntrySoundIds.Contains(t.SoundId));
            var updated = updatedEntryDto.Sounds.Where(t => existingEntrySoundIds.Contains(t.SoundId) && updatedEntrySoundIds.Contains(t.SoundId));

            // Process removed translations
            foreach (var sound in deleted)
            {
                var changeSet = await _sounds.Remove(sound.SoundId);
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

                var changeSet = await _sounds.Update(sound);
                changeSets.AddRange(changeSet);
            }

            return changeSets;
        }
        private async Task<List<ChangeSetModel>> ProcessTranslationsForEntryUpdate(EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            var changeSets = new List<ChangeSetModel>();

            var existingEntryTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedEntryTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var added = updatedEntryDto.Translations.Where(t => !existingEntryTranslationIds.Contains(t.TranslationId));
            var deleted = existingEntryDto.Translations.Where(t => !updatedEntryTranslationIds.Contains(t.TranslationId));
            var updated = updatedEntryDto.Translations.Where(t => existingEntryTranslationIds.Contains(t.TranslationId) && updatedEntryTranslationIds.Contains(t.TranslationId));

            // Process removed translations
            foreach (var translation in deleted)
            {
                var changeSet = await _translations.Remove(translation.TranslationId);
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

                var changeSet = await _translations.Update(translation);
                changeSets.AddRange(changeSet);
            }
            return changeSets;
        }
    }
}
