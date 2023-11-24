using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.sql.Services;
using chldr_data.sql.SqlEntities;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using chldr_data.DatabaseObjects.Interfaces;
using System.Collections.Immutable;
using System.Diagnostics;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("chldr_data.sql.tests")]

namespace chldr_data.sql.Repositories
{
    public class SqlEntriesRepository : SqlRepository<SqlEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        public async Task<List<EntryModel>> FindAsync(string inputText, FiltrationFlags? filtrationFlags = null)
        {
            try
            {
                IQueryable<SqlEntry> entries = _dbContext.Entries;
                if (filtrationFlags != null && filtrationFlags.EntryFilters != null)
                {
                    entries = IEntriesRepository.ApplyEntryFilters(_dbContext.Entries, filtrationFlags.EntryFilters);
                }

                var foundEntries = IEntriesRepository.Find(entries, inputText);

                // TODO: Optimize this method
                var entryModels = IEntriesRepository.GroupWithSubentries(_dbContext.Entries, foundEntries, FromEntry);

                if (filtrationFlags != null && filtrationFlags.TranslationFilters != null)
                {
                    entryModels = ITranslationsRepository.ApplyTranslationFilters(entryModels, filtrationFlags.TranslationFilters);
                }
                return entryModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Get and Take
        public override async Task<EntryModel> GetAsync(string entityId)
        {
            /*
             * This method returns exact entry that was requested, even if that's a subentry without a translation
             * Unlike other methods, it will not return its parent, rather only the specified entry
             */

            return FromEntry(entityId);
        }

        public async Task<List<EntryModel>> GetByIdsAsync(List<string> entryIds, FiltrationFlags? filtrationFlags = null)
        {
            IQueryable<SqlEntry> entries = _dbContext.Entries;
            if (filtrationFlags != null && filtrationFlags.EntryFilters != null)
            {
                entries = IEntriesRepository.ApplyEntryFilters(_dbContext.Entries, filtrationFlags.EntryFilters);
            }

            var filteredEntries = entries.Where(e => entryIds.Contains(e.EntryId));

            var models = IEntriesRepository.GroupWithSubentries(_dbContext.Entries, filteredEntries, FromEntry);

            // TODO: Move to a new ApplyTranslationFilters method
            foreach (var model in models)
            {
                model.Translations.RemoveAll(t => !t.LanguageCode.Equals("RUS"));
                foreach (var submodel in model.SubEntries)
                {
                    submodel.Translations.RemoveAll(t => !t.LanguageCode.Equals("RUS"));
                }
            }

            return models;
        }

        public override async Task<List<EntryModel>> TakeAsync(int offset, int limit)
        {
            var filteredEntries = _dbContext.Entries
                .OrderBy(e => e.RawContents)
                .Skip(offset)
                .Take(limit);

            return IEntriesRepository.GroupWithSubentries(_dbContext.Entries, filteredEntries, FromEntry);
        }
        public async Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags)
        {
            var filteredEntries = IEntriesRepository.ApplyEntryFilters(_dbContext.Entries, filtrationFlags?.EntryFilters);
            filteredEntries = filteredEntries
                      .OrderBy(e => e.RawContents)
                      .Skip(offset)
                      .Take(limit);

            return IEntriesRepository.GroupWithSubentries(_dbContext.Entries, filteredEntries, FromEntry);
        }
        public override async Task<List<EntryModel>> GetRandomsAsync(int limit)
        {
            var filteredEntries = IEntriesRepository.GetRandomEntries(_dbContext.Entries, limit);

            return IEntriesRepository.GroupWithSubentries(_dbContext.Entries, filteredEntries, FromEntry);
        }

        public async Task<List<EntryModel>> GetLatestEntriesAsync(int count)
        {
            var filteredEntries = _dbContext.Entries
                .OrderByDescending(e => e.CreatedAt)
                .Take(count);

            return IEntriesRepository.GroupWithSubentries(_dbContext.Entries, filteredEntries, FromEntry);
        }

        public async Task<List<EntryModel>> GetEntriesOnModerationAsync()
        {
            var count = 50;

            var filteredEntries = _dbContext.Entries
                .Where(entry => entry.Rate < UserModel.MemberRateRange.Lower)
                .Take(count);

            return IEntriesRepository.GroupWithSubentries(_dbContext.Entries, filteredEntries, FromEntry);
        }
        public async Task<List<EntryModel>> GetChildEntriesAsync(string entryId)
        {
            var entries = _dbContext.Entries.Where(e => e.ParentEntryId != null && e.ParentEntryId.Equals(entryId));
            return await entries.Select(e => FromEntry(e.EntryId)).ToListAsync();
        }

        #endregion

        #region Update methods
        public override async Task<List<ChangeSetModel>> AddAsync(EntryDto newEntryDto)
        {
            var user = UserModel.FromEntity(await _dbContext.Users.FindAsync(_userId) as SqlUser);

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
        public override async Task<List<ChangeSetModel>> UpdateAsync(EntryDto updatedEntryDto)
        {
            var resultingChangeSets = new List<ChangeSetModel>();

            try
            {
                var entry = await GetAsync(updatedEntryDto.EntryId);
                var user = UserModel.FromEntity(await _dbContext.Users.FindAsync(_userId) as SqlUser);

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

            var existingEntrySoundIds = existingEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();
            var updatedEntrySoundIds = updatedEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();

            var added = updatedEntryDto.Sounds.Where(t => !existingEntrySoundIds.Contains(t.SoundId));
            var deleted = existingEntryDto.Sounds.Where(t => !updatedEntrySoundIds.Contains(t.SoundId));
            var existingAndUpdated = updatedEntryDto.Sounds.Where(t => existingEntrySoundIds.Contains(t.SoundId) && updatedEntrySoundIds.Contains(t.SoundId));

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

            var existingEntryTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedEntryTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var added = updatedEntryDto.Translations.Where(t => !existingEntryTranslationIds.Contains(t.TranslationId));
            var deleted = existingEntryDto.Translations.Where(t => !updatedEntryTranslationIds.Contains(t.TranslationId));
            var existingAndUpdated = updatedEntryDto.Translations.Where(t => existingEntryTranslationIds.Contains(t.TranslationId) && updatedEntryTranslationIds.Contains(t.TranslationId));

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
            var user = UserModel.FromEntity(userEntity as SqlUser);
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
            var entries = IEntriesRepository.ApplyEntryFilters(_dbContext.Entries, filtrationFlags?.EntryFilters);
            var count = await entries.CountAsync();

            return count;
        }

        public EntryModel FromEntry(string entryId)
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
                .FirstOrDefault(e => e.EntryId.Equals(entryId));

            if (entry == null)
            {
                throw new NullReferenceException();
            }

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
           SqlContext context,
            FileService fileService,
            ExceptionHandler exceptionHandler,
            ITranslationsRepository translationsRepository,
            IPronunciationsRepository soundsRepository,
            string userId) : base(context, fileService, userId)
        {
            _exceptionHandler = exceptionHandler;
            _translations = (SqlTranslationsRepository)translationsRepository;
            _sounds = (SqlPronunciationsRepository)soundsRepository;
        }
    }
}
