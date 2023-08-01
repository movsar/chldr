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
using chldr_utils.Models;
using chldr_data.local.Services;

namespace chldr_data.Repositories
{
    public class RealmEntriesRepository : RealmRepository<RealmEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        public event Action<SearchResultModel>? GotNewSearchResult;
        private readonly RealmTranslationsRepository _translations;
        private readonly RealmSoundsRepository _sounds;

        public RealmEntriesRepository(
            ExceptionHandler exceptionHandler,
            FileService fileService,
            RequestService requestService,
            string userId,
            RealmTranslationsRepository translationsRepository,
            RealmSoundsRepository soundsRepository) : base(exceptionHandler, fileService, requestService, userId)
        {
            _translations = translationsRepository;
            _sounds = soundsRepository;
        }
        protected override RecordType RecordType => RecordType.Entry;
        protected override EntryModel FromEntityShortcut(RealmEntry entry)
        {
            var subEntries = _dbContext.All<RealmEntry>()
                .Where(e => e.ParentEntryId != null && e.ParentEntryId.Equals(entry.EntryId))
                .ToImmutableList();

            if (subEntries.Count() == 0)
            {
                return EntryModel.FromEntity(entry, entry.Source, entry.Translations, entry.Sounds);
            }
            else
            {
                return FromEntityShortcut(entry, subEntries);
            }
        }
        protected EntryModel FromEntityShortcut(RealmEntry entry, ImmutableList<RealmEntry> subEntries)
        {
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

                return entries.Select(e => FromEntityShortcut(e)).ToList();
            });

            return results;
        }
        public List<EntryModel> GetLatestEntries()
        {
            var entries = _dbContext.All<RealmEntry>().AsEnumerable().OrderByDescending(e => e.CreatedAt).Take(100);

            var entriesToReturn = entries.Select(entry => FromEntityShortcut(entry));

            return entriesToReturn.ToList();
        }
        public List<EntryModel> GetEntriesOnModeration()
        {
            var entries = _dbContext.All<RealmEntry>().AsEnumerable()
                .Where(entry => entry.Rate < UserModel.EnthusiastRateRange.Lower)
                .Take(50)
                .Select(entry => FromEntityShortcut(entry))
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
        public override async Task<List<ChangeSetModel>> Add(EntryDto newEntryDto)
        {
            if (newEntryDto == null || string.IsNullOrEmpty(newEntryDto.EntryId))
            {
                throw new NullReferenceException();
            }

            // Insert remote entity with translations
            var response = await _requestService.AddEntry(_userId, newEntryDto);
            if (!response.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }
            var responseData = RequestResult.GetData<InsertResponse>(response);
            if (responseData.CreatedAt == DateTimeOffset.MinValue)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }

            // Insert Entry entity with translations
            RealmEntry entry = null!;

            _dbContext.Write((Action)(() =>
            {
                newEntryDto.CreatedAt = responseData.CreatedAt;
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

                foreach (var changeSet in responseData.ChangeSets)
                {
                    var realmChangeSet = RealmChangeSet.FromDto((ChangeSetDto)changeSet, _dbContext);
                    _dbContext.Add<RealmChangeSet>(realmChangeSet);
                }
            }));

            return responseData.ChangeSets.Select(ChangeSetModel.FromDto).ToList();
        }
        public override async Task<List<ChangeSetModel>> Update(EntryDto updatedEntryDto)
        {
            var existingEntry = await GetAsync(updatedEntryDto.EntryId);
            var existingEntryDto = EntryDto.FromModel(existingEntry);

            // Update remote entity
            var response = await _requestService.UpdateEntry(_userId, updatedEntryDto);
            if (!response.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }
            var responseData = RequestResult.GetData<UpdateResponse>(response);

            // Update local entity
            _dbContext.Write(() =>
            {
                RealmEntry.FromDto(updatedEntryDto, _dbContext);

                foreach (var changeSet in responseData.ChangeSets)
                {
                    var realmChangeSet = RealmChangeSet.FromDto(changeSet, _dbContext);
                    _dbContext.Add(realmChangeSet);
                }
            });

            return responseData.ChangeSets.Select(ChangeSetModel.FromDto).ToList();
        }
        public override async Task<List<ChangeSetModel>> Remove(string entityId)
        {
            // Remove remote entity
            var response = await _requestService.RemoveEntry(_userId, entityId);
            if (!response.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }
            var responseData = RequestResult.GetData<UpdateResponse>(response);

            // Remove local entity
            var entry = _dbContext.Find<RealmEntry>(entityId);
            if (entry == null)
            {
                return new List<ChangeSetModel>();
            }

            var sounds = entry.Sounds.Select(s => s.SoundId).ToArray();
            var translations = entry.Translations.Select(t => t.TranslationId).ToArray();

            await _sounds.RemoveRange(sounds);
            await _translations.RemoveRange(translations);

            _dbContext.Write(() =>
            {
                _dbContext.Remove(entry);

                foreach (var changeSet in responseData.ChangeSets)
                {
                    _dbContext.Add(RealmChangeSet.FromDto(changeSet, _dbContext));
                }
            });

            return responseData.ChangeSets.Select(ChangeSetModel.FromDto).ToList();
        }

        public Task<ChangeSetModel> Promote(IEntry entry)
        {
            throw new NotImplementedException();
        }
    }
}