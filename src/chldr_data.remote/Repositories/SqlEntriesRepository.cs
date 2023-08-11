﻿using chldr_data.DatabaseObjects.Dtos;
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
        private async Task<List<SqlEntry>> GroupEntriesAsync(IQueryable<SqlEntry> entries)
        {
            /*
             * This method looks for entries that have parents, and takes the parents instead
             * with children added as subentries.
             * 
             * Should be called only on results after all the filtration has been done.
             * 
             * @param entries = all entries from database
             * @param resultingEntries = those that have already been filtered by some criteria
            */

            var subEntryParentIds = await entries.Where(e => e.ParentEntryId != null).Select(e => e.ParentEntryId).ToArrayAsync();

            // Add subEntry parents
            var parents = _dbContext.Entries.Where(e => subEntryParentIds.Contains(e.EntryId));
            var resultingEntries = await entries.Union(parents).ToListAsync();

            // Get standalone entry ids
            var subEntryIds = resultingEntries.Where(e => e.ParentEntryId != null).Select(e => e.EntryId).ToList();

            // Remove standalone sub entries
            resultingEntries.RemoveAll(e => subEntryIds.Contains(e.EntryId));

            return resultingEntries;
        }

        private async Task<IQueryable<TEntity>> ApplyFiltrationFlags<TEntity>(IQueryable<TEntity> sourceEntries, FiltrationFlags filtrationFlags)
            where TEntity : IEntryEntity
        {
            // Leave only selected entry types
            var entryTypes = filtrationFlags.EntryTypes.Select(et => (int)et).ToArray();
            var resultingEntries = sourceEntries.Where(e => entryTypes.Contains(e.Type));

            // If StartsWith specified, remove all that don't start with that string
            if (!string.IsNullOrWhiteSpace(filtrationFlags.StartsWith))
            {
                var str = filtrationFlags.StartsWith.ToLower();
                resultingEntries = resultingEntries.Where(e => e.RawContents.StartsWith(str));
            }

            // Don't include entries on moderation, if not specified otherwise
            if (!filtrationFlags.IncludeOnModeration)
            {
                resultingEntries = resultingEntries.Where(e => e.Rate > UserModel.MemberRateRange.Upper);
            }

            return resultingEntries;
        }

        #region Search
        protected async Task<List<SqlEntry>> FinderAsync(IQueryable<SqlEntry> query, string inputText)
        {
            // This is the search algorythm, it will be exactly the same for both Sql and Realm databases
            // It takes the entity interfaces and returns ids, no DB specific operations or fields

            // @Nurmagomed - this is your domain :)

            IQueryable<SqlEntry> resultingQuery;
            if (string.IsNullOrEmpty(inputText) || inputText.Length <= 3)
            {
                resultingQuery = query
                    .Where(e => e.RawContents.Equals(inputText, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                resultingQuery = query
                    .Where(e => e.RawContents.StartsWith(inputText, StringComparison.OrdinalIgnoreCase));
            }

            var resultingEntries = await GroupEntriesAsync(resultingQuery);
            return resultingEntries;
        }

        public async Task<List<EntryModel>> FindAsync(string inputText)
        {
            var result = new List<EntryModel>();

            try
            {
                var matchingEntries = await FinderAsync(_dbContext.Entries, inputText);

                // Apply the shortcut method to the matching entry IDs            
                result = matchingEntries.Select(e => FromSqlEntry(e.EntryId)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        #endregion

        #region Get and Take
        public override async Task<EntryModel> GetAsync(string entityId)
        {
            /*
             * This method returns exact entry that was requested, even if that's a subentry without a translation
             * Unlike other methods, it will not return its parent, rather only the specified entry
             */

            return FromSqlEntry(entityId);
        }

        public async Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags)
        {
            var filteredEntries = await ApplyFiltrationFlags(_dbContext.Entries, filtrationFlags);
            filteredEntries = filteredEntries
                      .OrderBy(e => e.RawContents)
                      .Skip(offset)
                      .Take(limit);

            var results = await GroupEntriesAsync(filteredEntries);
            return results.Select(e => FromSqlEntry(e.EntryId)).ToList();
        }
        public override async Task<List<EntryModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();

            // Fetch all EntryIds from the database
            var ids = await _dbContext.Set<SqlEntry>().Where(e => e.ParentEntryId == null).Select(e => e.EntryId).ToListAsync();

            // Shuffle the ids and take what's needed
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            // Fetch entries with the selected EntryIds
            var filteredEntries = _dbContext.Entries
                .Where(entry => randomlySelectedIds.Contains(entry.EntryId))
                .AsNoTracking();

            var results = await GroupEntriesAsync(filteredEntries);
            return results.Select(e => FromSqlEntry(e.EntryId)).ToList();
        }

        public async Task<List<EntryModel>> GetLatestEntriesAsync()
        {
            var count = 50;

            var filteredEntries = _dbContext.Entries
                .OrderByDescending(e => e.CreatedAt)
                .Take(count);

            var results = await GroupEntriesAsync(filteredEntries);
            return results.Select(e => FromSqlEntry(e.EntryId)).ToList();
        }

        public async Task<List<EntryModel>> GetEntriesOnModerationAsync()
        {
            var count = 50;

            var filteredEntries = _dbContext.Entries
                .Where(entry => entry.Rate < UserModel.MemberRateRange.Lower)
                .Take(count);

            var results = await GroupEntriesAsync(filteredEntries);
            return results.Select(e => FromSqlEntry(e.EntryId)).ToList();
        }
        public async Task<List<EntryModel>> GetChildEntriesAsync(string entryId)
        {
            var entries = _dbContext.Entries.Where(e => e.ParentEntryId != null && e.ParentEntryId.Equals(entryId));
            return await entries.Select(e => FromSqlEntry(e.EntryId)).ToListAsync();
        }
        public override async Task<List<EntryModel>> TakeAsync(int offset, int limit)
        {
            var entries = await _dbContext.Entries
                .OrderBy(e => e.RawContents)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return entries.Select(e => FromSqlEntry(e.EntryId)).ToList();
        }
        #endregion

        #region Update methods
        public override async Task<List<ChangeSetModel>> AddAsync(EntryDto newEntryDto)
        {
            var user = UserModel.FromEntity(await _dbContext.Users.FindAsync(_userId));
         
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
        public override async Task<List<ChangeSetModel>> UpdateAsync(EntryDto updatedEntryDto)
        {
            var resultingChangeSets = new List<ChangeSetModel>();

            try
            {
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
        public override async Task<List<ChangeSetModel>> RemoveAsync(string entryId)
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
                    var translationChangeSet = await _translations.RemoveAsync(translationId);
                    changeSets.AddRange(translationChangeSet);
                }

                // Process removed sounds
                foreach (var soundId in soundIds)
                {

                    var translationChangeSet = await _sounds.RemoveAsync(soundId);
                    changeSets.AddRange(translationChangeSet);
                }

                // Process removed entry
                var changeSet = await base.RemoveAsync(entryId);
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
                var changeSet = await _sounds.AddAsync(sound);
                changeSets.AddRange(changeSet);
            }

            // Process removed sounds
            foreach (var sound in deleted)
            {
                var changeSet = await _sounds.RemoveAsync(sound.SoundId);
                changeSets.AddRange(changeSet);
            }

            // Process updated sounds
            foreach (var sound in existingAndUpdated)
            {
                var changeSet = await _sounds.UpdateAsync(sound);
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
                var changeSet = await _translations.AddAsync(translation);
                changeSets.AddRange(changeSet);
            }

            // Process removed translations
            foreach (var translation in deleted)
            {
                var changeSet = await _translations.RemoveAsync(translation.TranslationId);
                changeSets.AddRange(changeSet);
            }

            // Process updated translations
            foreach (var translation in existingAndUpdated)
            {
                var changeSet = await _translations.UpdateAsync(translation);
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
        #endregion

        public async Task<int> CountAsync(FiltrationFlags filtrationFlags)
        {
            var entries = await ApplyFiltrationFlags(_dbContext.Entries, filtrationFlags);
            var count = await entries.CountAsync();

            return count;
        }

        private EntryModel FromSqlEntry(string entryId)
        {
            /*
             * This retrieves the entry with all its dependencies, because of the EF Core, we need to specify
             * which related objects are going to be retrieved, so this method basically fills the entity
             * After that, it goes on to build a model, to allow multiple database implementations it breaks down the
             * relationships and passes all the related objects to EntryMode.FromEntity method
             */

            var entry = _dbContext.Entries
                .Include(e => e.Source)
                .Include(e => e.User)
                .Include(e => e.Translations)
                .Include(e => e.Sounds)
                .First(e => e.EntryId.Equals(entryId));

            var subEntries = _dbContext.Entries
             .Where(e => e.ParentEntryId != null && e.ParentEntryId.Equals(entry.EntryId))
             .Include(e => e.Source)
             .Include(e => e.User)
             .Include(e => e.Translations)
             .Include(e => e.Sounds)
             .ToImmutableList();

            if (!subEntries.Any())
            {
                return EntryModel.FromEntity(entry, entry.Source, entry.Translations, entry.Sounds);
            }

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

        private readonly ExceptionHandler _exceptionHandler;
        private readonly SqlTranslationsRepository _translations;
        private readonly SqlPronunciationsRepository _sounds;
        protected override RecordType RecordType => RecordType.Entry;

        public SqlEntriesRepository(
            DbContextOptions<SqlContext> dbConfig,
            FileService fileService,
            ExceptionHandler exceptionHandler,
            ITranslationsRepository translationsRepository,
            IPronunciationsRepository soundsRepository,
            string userId) : base(dbConfig, fileService, userId)
        {
            _exceptionHandler = exceptionHandler;
            _translations = (SqlTranslationsRepository)translationsRepository;
            _sounds = (SqlPronunciationsRepository)soundsRepository;
        }
    }
}
