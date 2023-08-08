using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.local.Repositories;
using System.Collections.Immutable;
using chldr_data.Responses;
using chldr_data.Services;
using chldr_data.Enums.WordDetails;
using chldr_data.local.Services;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.Repositories
{
    public class RealmEntriesRepository : RealmRepository<RealmEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        public event Action<SearchResultModel>? GotNewSearchResult;

        public RealmEntriesRepository(
            ExceptionHandler exceptionHandler,
            FileService fileService,
            string userId) : base(exceptionHandler, fileService, userId){}

        protected override RecordType RecordType => RecordType.Entry;
        protected override EntryModel FromEntity(RealmEntry entry)
        {
            return EntryModel.FromEntity(entry, entry.Source, entry.Translations, entry.Sounds);
        }
        protected EntryModel FromEntityWithSubEntries(RealmEntry entry)
        {
            var subEntries = _dbContext.All<RealmEntry>()
                   .Where(e => e.ParentEntryId != null && e.ParentEntryId.Equals(entry.EntryId))
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

                return entries.Select(FromEntityWithSubEntries).ToList();
            });

            return results;
        }
        public List<EntryModel> GetLatestEntries()
        {
            var entries = _dbContext.All<RealmEntry>().AsEnumerable().OrderByDescending(e => e.CreatedAt).Take(100);

            var entriesToReturn = entries.Select(FromEntityWithSubEntries);

            return entriesToReturn.ToList();
        }
        public List<EntryModel> GetEntriesOnModeration()
        {
            var entries = _dbContext.All<RealmEntry>().AsEnumerable()
                .Where(entry => entry.Rate < UserModel.EnthusiastRateRange.Lower)
                .Take(50)
                .Select(FromEntityWithSubEntries)
                .ToList();

            return entries;
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

        public async Task<List<EntryModel>> FindAsync(string inputText, FiltrationFlags filtrationFlags)
        {
            var result = new List<EntryModel>();

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
                    var soundDto = newEntryDto.SoundDtos.FirstOrDefault<SoundDto>(s => s.SoundId == sound.SoundId && !string.IsNullOrEmpty(s.RecordingB64));
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

        public async Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags)
        {
            var entries = IEntriesRepository.GetFilteredEntries(_dbContext.All<RealmEntry>(), filtrationFlags);

            var groupedEntries = await entries
                .OrderBy(e => e.RawContents)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return groupedEntries.Select(FromEntityWithSubEntries).ToList();
        }

        public async Task<int> CountAsync(FiltrationFlags filtrationFlags)
        {
            var entries = IEntriesRepository.GetFilteredEntries(_dbContext.All<RealmEntry>(), filtrationFlags);

            return await entries.CountAsync();
        }
    }
}