using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.realm.RealmEntities;
using chldr_data.Models;
using System.Collections.Immutable;
using chldr_data.Services;
using chldr_data.realm.Services;
using Realms;
using chldr_data.Interfaces;

namespace chldr_data.Repositories
{
    public class RealmEntriesRepository : RealmRepository<RealmEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        public List<EntryModel> GroupWithSubentries(IQueryable<IEntryEntity> filteredEntries, Func<string, EntryModel> fromEntity)
        {
            // Convert filteredEntries to a list to prevent multiple enumeration
            var filteredEntriesList = filteredEntries.ToList();

            // Collect parent entry IDs
            var subEntryParentIds = new HashSet<string>();
            foreach (var entry in filteredEntriesList)
            {
                if (entry.ParentEntryId != null)
                {
                    subEntryParentIds.Add(entry.ParentEntryId);
                }
            }

            // Filter parents from Realm database using Filter
            var parents = new List<IEntryEntity>();
            foreach (var parentId in subEntryParentIds)
            {
                var parent = _dbContext.All<RealmEntry>().Where(e => e.EntryId == parentId).FirstOrDefault();
                if (parent != null)
                {
                    parents.Add(parent);
                }
            }

            // Combine filtered entries with parents
            var combinedEntries = new List<IEntryEntity>(filteredEntriesList);
            combinedEntries.AddRange(parents);

            // Collect subentry IDs
            var subEntryIds = new HashSet<string>();
            foreach (var entry in combinedEntries)
            {
                if (entry.ParentEntryId != null)
                {
                    subEntryIds.Add(entry.EntryId);
                }
            }

            // Remove standalone subentries
            combinedEntries.RemoveAll(e => subEntryIds.Contains(e.EntryId));

            // Convert to EntryModel
            var models = combinedEntries.Select(e => fromEntity(e.EntryId)).ToList();

            return models;
        }

        #region Get and Take
        public override async Task<List<EntryModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random(DateTime.Now.Millisecond);

            var results = await Task.Run(() =>
            {
                var entries = _dbContext.All<RealmEntry>()
                     .Where(e => e.ParentEntryId == null)
                     .Where(e => e.Rate > 0)
                     .AsEnumerable()
                     .OrderBy(entry => entry.GetHashCode())
                     .OrderBy(x => randomizer.Next(0, Constants.EntriesApproximateCoount))
                     .Take(limit);

                return entries.Select(FromEntity).ToList();
            });

            return results;
        }
        public async Task<List<EntryModel>> GetLatestEntriesAsync(int count)
        {
            var entries = _dbContext.All<RealmEntry>().AsEnumerable().OrderByDescending(e => e.CreatedAt).Take(count);

            var entriesToReturn = entries.Select(FromEntity);

            return entriesToReturn.ToList();
        }
        public async Task<List<EntryModel>> GetEntriesOnModerationAsync()
        {
            var entries = _dbContext.All<RealmEntry>().AsEnumerable()
                .Where(entry => entry.Rate < UserModel.EnthusiastRateRange.Lower)
                .Take(50)
                .Select(FromEntity)
                .ToList();

            return entries;
        }

        public async Task<List<EntryModel>> FindAsync(string inputText, FiltrationFlags? filtrationFlags = null)
        {
            var result = new List<EntryModel>();
            if (inputText == null)
            {
                return result;
            }

            if (inputText.Length <= 3)
            {
                result.AddRange(RealmSearchHelper.DirectSearch(_dbContext, inputText, RealmSearchHelper.StartsWithFilter(inputText), 50));
            }
            else if (inputText.Length > 3)
            {
                result.AddRange(RealmSearchHelper.DirectSearch(_dbContext, inputText, RealmSearchHelper.EntryFilter(inputText), 50));
                result.AddRange(RealmSearchHelper.ReverseSearch(_dbContext, inputText, RealmSearchHelper.TranslationFilter(inputText), 50));
            }

            // Sort
            SearchServiceHelper.PostProcessing(inputText, result);

            return result;
        }
        public async Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags)
        {
            var filteredEntries = ApplyEntryFilters(_dbContext.All<RealmEntry>(), filtrationFlags?.EntryFilters);
            var filteredEntriesAsQueriable = filteredEntries
                      .OrderBy(e => e.RawContents)
                      .Skip(offset)
                      .Take(limit)
                      .AsQueryable();

            return GroupWithSubentries(filteredEntriesAsQueriable, FromEntry);
        }

        #endregion

        #region Update
        public override async Task Add(EntryDto newEntryDto)
        {
            // Insert Entry entity with translations
            RealmEntry entry = null!;

            _dbContext.Write((Action)(() =>
            {
                entry = RealmEntry.FromDto(newEntryDto, _dbContext);
                _dbContext.Add<RealmEntry>(entry);

                // Save audiofiles if any
                foreach (var sound in entry.Sounds)
                {
                    var soundDto = newEntryDto.Sounds.FirstOrDefault<PronunciationDto>(s => s.SoundId == sound.SoundId && !string.IsNullOrEmpty(s.RecordingB64));
                    if (soundDto == null)
                    {
                        continue;
                    }

                    var filePath = Path.Combine(_fileService.EntrySoundsDirectory, soundDto.FileName);
                    File.WriteAllText(filePath, soundDto.RecordingB64);
                }
            }));
        }
        public override async Task Update(EntryDto updatedEntryDto)
        {
            _dbContext.Write(() =>
            {
                RealmEntry.FromDto(updatedEntryDto, _dbContext);
            });
        }
        public override async Task Remove(string entityId)
        {
            var entry = _dbContext.Find<RealmEntry>(entityId);
            if (entry == null)
            {
                throw new NullReferenceException("No such entity could be found");
            }

            _dbContext.Write(() =>
            {
                _dbContext.Remove(entry);
            });
        }

        public Task<ChangeSetModel> Promote(IEntry entry)
        {
            throw new NotImplementedException();
        }
        #endregion
        public static List<EntryModel> ApplyTranslationFilters(List<EntryModel> entryModels, TranslationFilters filters)
        {
            // Prepare language codes to look for
            var languageCodes = new HashSet<string>();
            if (filters.LanguageCodes != null)
            {
                foreach (var code in filters.LanguageCodes)
                {
                    languageCodes.Add(code.ToLower().Trim());
                }
            }

            foreach (var entry in entryModels.ToList())
            {
                var translationsToKeep = new List<TranslationModel>();

                foreach (var translation in entry.Translations)
                {
                    var languageCodeLower = translation.LanguageCode.ToLower();
                    bool isLanguageMatch = languageCodes.Count == 0 || languageCodes.Contains(languageCodeLower);
                    bool isRateMatch = !(filters.IncludeOnModeration == false && translation.Rate <= UserModel.MemberRateRange.Upper);

                    if (isLanguageMatch && isRateMatch)
                    {
                        translationsToKeep.Add(translation);
                    }
                }

                entry.Translations = translationsToKeep;
            }

            entryModels = entryModels.Where(e => e.Translations.Any()).ToList();
            return entryModels;
        }

        public IEnumerable<RealmEntry> ApplyEntryFilters(IQueryable<RealmEntry> sourceEntries, EntryFilters? entryFilters)
        {
            // Leave only selected entry types
            var resultingEntries = sourceEntries.AsQueryable();
            if (entryFilters.EntryTypes != null && entryFilters.EntryTypes.Any())
            {
                var entryTypeQueries = entryFilters.EntryTypes.Select(et => $"Type == {(int)et}");
                var combinedQuery = string.Join(" OR ", entryTypeQueries);
                resultingEntries = resultingEntries.Filter(combinedQuery);
            }

            // If StartsWith specified, remove all that don't start with that string
            if (!string.IsNullOrWhiteSpace(entryFilters.StartsWith))
            {
                var str = entryFilters.StartsWith.ToLower();
                resultingEntries = resultingEntries.Filter($"RawContents BEGINSWITH[c] '{str}'");
            }

            // Don't include entries on moderation, if not specified otherwise
            if (entryFilters.IncludeOnModeration != null && entryFilters.IncludeOnModeration == false)
            {
                var upperRate = UserModel.MemberRateRange.Upper;
                resultingEntries = resultingEntries.Filter($"Rate > '{upperRate}'");
            }

            return resultingEntries;
        }
        public async Task<int> CountAsync(FiltrationFlags filtrationFlags)
        {
            var a = _dbContext.All<RealmEntry>().Where(e => e.Content.StartsWith("а"));

            var entries = ApplyEntryFilters(_dbContext.All<RealmEntry>(), filtrationFlags.EntryFilters);

            return entries.TryGetNonEnumeratedCount(out var count) ? count : entries.Count();
        }

        public override EntryModel FromEntity(RealmEntry entry)
        {
            var subEntries = _dbContext.All<RealmEntry>()
                   .Where(e => e.ParentEntryId != null && e.ParentEntryId.Equals(entry.EntryId))
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

        public EntryModel FromEntry(string entryId)
        {
            return FromEntity(_dbContext.All<RealmEntry>().First(e => e.EntryId.Equals(entryId)));
        }

        public Task<List<EntryModel>> GetByIdsAsync(List<string> entryIds, FiltrationFlags? filtrationFlags = null)
        {
            throw new NotImplementedException();
        }

        protected override RecordType RecordType => RecordType.Entry;

        public RealmEntriesRepository(
            IExceptionHandler exceptionHandler,
            IFileService fileService,
            string userId) : base(exceptionHandler, fileService, userId) { }


    }
}