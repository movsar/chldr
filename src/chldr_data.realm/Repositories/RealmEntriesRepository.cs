using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.realm.RealmEntities;
using chldr_data.Models;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.realm.Repositories;
using System.Collections.Immutable;
using chldr_data.Responses;
using chldr_data.Services;
using chldr_data.Enums.WordDetails;
using chldr_data.realm.Services;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.Repositories
{
    public class RealmEntriesRepository : RealmRepository<RealmEntry, EntryModel, EntryDto>, IEntriesRepository
    {
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
            //var entries = await ApplyFiltrationFlags(_dbContext.All<RealmEntry>(), filtrationFlags);

            //var groupedEntries = await entries
            //    .OrderBy(e => e.RawContents)
            //    .Skip(offset)
            //    .Take(limit)
            //    .ToListAsync();

            //return groupedEntries.Select(FromEntityWithSubEntries).ToList();

            throw new NotImplementedException();
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

        public async Task<int> CountAsync(FiltrationFlags filtrationFlags)
        {
            //var entries = await IEntriesRepository.ApplyFiltrationFlags(_dbContext.All<RealmEntry>(), filtrationFlags);
            //return await entries.CountAsync();

            throw new NotImplementedException();
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
            ExceptionHandler exceptionHandler,
            FileService fileService,
            string userId) : base(exceptionHandler, fileService, userId) { }


    }
}