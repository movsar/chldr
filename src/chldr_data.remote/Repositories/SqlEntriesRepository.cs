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
using chldr_data.Services;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_utils.Exceptions;
using System.Collections.Immutable;

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
            DbContextOptions<SqlContext> dbConfig,
            FileService fileService,
            ExceptionHandler exceptionHandler,
            ITranslationsRepository translationsRepository,
            ISoundsRepository soundsRepository,
            string userId) : base(dbConfig, fileService, userId)
        {
            _exceptionHandler = exceptionHandler;
            _translations = (SqlTranslationsRepository)translationsRepository;
            _sounds = (SqlSoundsRepository)soundsRepository;
        }

        protected override RecordType RecordType => RecordType.Entry;

        public override async Task<EntryModel> GetAsync(string entityId)
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

            return FromEntity(entry);
        }

        public async Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags)
        {
            var entries = IEntriesRepository.GetFilteredEntries(_dbContext.Entries, filtrationFlags);

            // Fetch all EntryIds from the database
            var topLevelEntryIds = await entries.Where(e => e.ParentEntryId == null).Select(e => e.EntryId).ToListAsync();

            var entities = await entries
                      .OrderBy(e => e.RawContents)
                      .Include(e => e.Source)
                      .Include(e => e.User)
                      .Include(e => e.Translations)
                      .Include(e => e.Sounds)

                      .Skip(offset)
                      .Take(limit)

                      .ToListAsync();

            return entities.Select(FromEntityWithSubEntries).ToList();
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
            var models = entities.Select(FromEntity).ToList();
            return models;
        }
        protected override EntryModel FromEntity(SqlEntry entry)
        {
            return EntryModel.FromEntity(entry, entry.Source, entry.Translations, entry.Sounds);
        }
        protected EntryModel FromEntityWithSubEntries(SqlEntry entry)
        {
            var subEntries = _dbContext.Entries
             .Where(e => e.ParentEntryId != null && e.ParentEntryId.Equals(entry.EntryId))
             .Include(e => e.Source)
             .Include(e => e.User)
             .Include(e => e.Translations)
             .Include(e => e.Sounds)
             .ToImmutableList();

            var subSources = subEntries.ToDictionary(e => e.EntryId, e => e.Source as ISourceEntity);
            var subSounds = subEntries.ToDictionary(e => e.EntryId, e => e.Sounds.ToList().Cast<ISoundEntity>());
            var subEntryTranslations = subEntries.ToDictionary(e => e.EntryId, e => e.Translations.ToList().Cast<ITranslationEntity>());

            return EntryModel.FromEntity(
                    entry,
                    entry.Source,
                    entry.Translations,
                    entry.Sounds,
                    subEntries.Cast<IEntryEntity>(),
                    subSources,
                    subEntryTranslations,
                    subSounds
                );
        }

        protected static async Task<List<string>> FinderAsync(IQueryable<IEntryEntity> query, string inputText)
        {
            // This is the search algorythm, it will be exactly the same for both Sql and Realm databases
            // It takes the entity interfaces and returns ids, no DB specific operations or fields

            // @Nurmagomed - this is your domain :)

            if (string.IsNullOrEmpty(inputText) || inputText.Length <= 3)
            {
                query = query
                    .Where(e => e.RawContents.Equals(inputText, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                query = query
                    .Where(e => e.RawContents.StartsWith(inputText, StringComparison.OrdinalIgnoreCase));
            }

            return await query
                .Select(e => e.EntryId)
                .ToListAsync();
        }

        public async Task<List<EntryModel>> FindAsync(string inputText, FiltrationFlags filtrationFlags)
        {
            var result = new List<EntryModel>();

            try
            {
                var matchingEntryIds = await FinderAsync(_dbContext.Entries.Cast<IEntryEntity>(), inputText);

                // Apply the shortcut method to the matching entry IDs            
                result = matchingEntryIds
                    .Select(id => FromEntityWithSubEntries(_dbContext.Entries
                        .Include(e => e.Source)
                        .Include(e => e.User)
                        .Include(e => e.Translations)
                        .Include(e => e.Sounds)
                        .AsNoTracking()
                        .First(e => e.EntryId.Equals(id))))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            // Sort etc
            SearchServiceHelper.PostProcessing(inputText, result);
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

            return entries.Select(FromEntityWithSubEntries).ToList();
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

            return entries.Select(FromEntityWithSubEntries).ToList();
        }
        private async Task<bool> ValidateParent(EntryDto entryDto)
        {
            // If Parent is specified, restrict to one level deep, parent child relationship, no hierarchies
            if (entryDto.ParentEntryId != null)
            {
                var parent = await GetAsync(entryDto.ParentEntryId);
                if (!string.IsNullOrEmpty(parent.ParentEntryId))
                {
                    return false;
                }

                var children = await GetChildEntriesAsync(entryDto.EntryId);
                if (children.Count() > 0)
                {
                    return false;
                }
            }

            return true;
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
                throw new UnauthorizedException();
            }

            if (!(await ValidateParent(newEntryDto)))
            {
                throw new InvalidArgumentsException("Error:Invalid_parent_entry");
            }

            // Set rate
            newEntryDto.Rate = user.GetRateRange().Lower;
            foreach (var translationDto in newEntryDto.TranslationsDtos)
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
                foreach (var sound in newEntryDto.SoundDtos)
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

        private async Task<List<EntryModel>> GetChildEntriesAsync(string entryId)
        {
            var entries = _dbContext.Entries.Where(e => e.ParentEntryId != null && e.ParentEntryId.Equals(entryId));
            return await entries.Select(e => EntryModel.FromEntity(e, e.Source, e.Translations, e.Sounds)).ToListAsync();
        }

        public override async Task<List<ChangeSetModel>> Update(EntryDto updatedEntryDto)
        {
            var resultingChangeSets = new List<ChangeSetModel>();

            try
            {
                if (!(await ValidateParent(updatedEntryDto)))
                {
                    throw new InvalidArgumentsException("Error:Invalid_parent_entry");
                }

                var entry = await GetAsync(updatedEntryDto.EntryId);
                var user = UserModel.FromEntity(await _dbContext.Users.FindAsync(_userId));

                var existingEntryDto = EntryDto.FromModel(entry);

                // Update entry
                var entryChanges = Change.GetChanges(updatedEntryDto, existingEntryDto);
                if (entryChanges.Count != 0 && user.CanEdit(entry.Rate, entry.UserId))
                {
                    var updatedEntryEntity = SqlEntry.FromDto(updatedEntryDto, _dbContext);

                    // Enforce proper Rate
                    updatedEntryDto.Rate = user.GetRateRange().Lower;

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
                var entry = await GetAsync(entryId);
                var user = UserModel.FromEntity(await _dbContext.Users.FindAsync(_userId));

                // Check whether the user has access to remove the entry with all its child items
                if (!user.CanRemove(entry.Rate, entry.UserId, entry.CreatedAt))
                {
                    throw new InvalidOperationException();
                }
                foreach (var item in entry.Translations)
                {
                    if (!user.CanRemove(item.Rate, item.UserId, item.CreatedAt))
                    {
                        throw new InvalidOperationException();
                    }
                }
                foreach (var item in entry.Sounds)
                {
                    if (!user.CanRemove(item.Rate, item.UserId, item.CreatedAt))
                    {
                        throw new InvalidOperationException();
                    }
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

            var existingEntrySoundIds = existingEntryDto.SoundDtos.Select(t => t.SoundId).ToHashSet();
            var updatedEntrySoundIds = updatedEntryDto.SoundDtos.Select(t => t.SoundId).ToHashSet();

            var added = updatedEntryDto.SoundDtos.Where(t => !existingEntrySoundIds.Contains(t.SoundId));
            var deleted = existingEntryDto.SoundDtos.Where(t => !updatedEntrySoundIds.Contains(t.SoundId));
            var existingAndUpdated = updatedEntryDto.SoundDtos.Where(t => existingEntrySoundIds.Contains(t.SoundId) && updatedEntrySoundIds.Contains(t.SoundId));

            // Process added sounds
            foreach (var sound in added)
            {
                var changeSet = await _sounds.Add(sound);
                changeSets.AddRange(changeSet);
            }

            // Process removed sounds
            foreach (var sound in deleted)
            {
                var changeSet = await _sounds.Remove(sound.SoundId);
                changeSets.AddRange(changeSet);
            }

            // Process updated sounds
            foreach (var sound in existingAndUpdated)
            {
                var changeSet = await _sounds.Update(sound);
                changeSets.AddRange(changeSet);
            }

            return changeSets;
        }
        private async Task<List<ChangeSetModel>> ProcessTranslationsForEntryUpdate(EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            var changeSets = new List<ChangeSetModel>();

            var existingEntryTranslationIds = existingEntryDto.TranslationsDtos.Select(t => t.TranslationId).ToHashSet();
            var updatedEntryTranslationIds = updatedEntryDto.TranslationsDtos.Select(t => t.TranslationId).ToHashSet();

            var added = updatedEntryDto.TranslationsDtos.Where(t => !existingEntryTranslationIds.Contains(t.TranslationId));
            var deleted = existingEntryDto.TranslationsDtos.Where(t => !updatedEntryTranslationIds.Contains(t.TranslationId));
            var existingAndUpdated = updatedEntryDto.TranslationsDtos.Where(t => existingEntryTranslationIds.Contains(t.TranslationId) && updatedEntryTranslationIds.Contains(t.TranslationId));

            // Process added translations
            foreach (var translation in added)
            {
                var changeSet = await _translations.Add(translation);
                changeSets.AddRange(changeSet);
            }

            // Process removed translations
            foreach (var translation in deleted)
            {
                var changeSet = await _translations.Remove(translation.TranslationId);
                changeSets.AddRange(changeSet);
            }

            // Process updated translations
            foreach (var translation in existingAndUpdated)
            {
                var changeSet = await _translations.Update(translation);
                changeSets.AddRange(changeSet);
            }
            return changeSets;
        }

        public async Task<ChangeSetModel> Promote(IEntry entryInfo)
        {
            var entryEntity = await _dbContext.Entries
                .Include(e => e.Translations)
                .Include(e => e.Sounds)
                .FirstOrDefaultAsync(e => e.EntryId.Equals(entryInfo.EntryId));

            if (entryEntity == null)
            {
                throw new NullReferenceException();
            }

            var userEntity = await _dbContext.Users.FindAsync(_userId);
            var user = UserModel.FromEntity(userEntity);
            var newRate = user.GetRateRange().Lower;

            entryEntity.Rate = newRate;
            await _dbContext.SaveChangesAsync();

            var changes = new List<Change>() { new Change() { OldValue = entryEntity.Rate, NewValue = newRate, Property = "Rate" } };
            var changeSet = CreateChangeSetEntity(Operation.Update, entryEntity.EntryId, changes);
            _dbContext.ChangeSets.Add(changeSet);
            await _dbContext.SaveChangesAsync();

            foreach (var translation in entryEntity.Translations)
            {
                translation.Rate = newRate;
            }

            await _dbContext.SaveChangesAsync();

            return ChangeSetModel.FromEntity(changeSet);
        }

        public async Task<int> CountAsync(FiltrationFlags filtrationFlags)
        {
            var count = await IEntriesRepository.GetFilteredEntries(_dbContext.Entries, filtrationFlags).CountAsync();

            return count;
        }
    }
}
