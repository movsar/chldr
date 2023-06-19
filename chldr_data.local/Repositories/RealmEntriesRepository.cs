using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_utils.Interfaces;
using chldr_utils;
using GraphQL;
using Realms;
using Microsoft.EntityFrameworkCore.Update;
using chldr_utils.Services;

namespace chldr_data.Repositories
{
    public class RealmEntriesRepository : RealmRepository<RealmEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        private readonly ITranslationsRepository _translations;
        private readonly ISoundsRepository _sounds;

        public RealmEntriesRepository(
            Realm context,
            ExceptionHandler exceptionHandler,
            ITranslationsRepository translationsRepository,
            ISoundsRepository soundsRepository) : base(context, exceptionHandler)
        {
            _translations = translationsRepository;
            _sounds = soundsRepository;
        }
        protected override RecordType RecordType => RecordType.Entry;
        protected override EntryModel FromEntityShortcut(RealmEntry entry)
        {
            return EntryModel.FromEntity(
                                    entry,
                                    entry.Source,
                                    entry.Translations,
                                    entry.Sounds);
        }

        public override void Add(EntryDto newEntryDto)
        {
            RealmEntry? newEntry = null;

            _dbContext.Write(() =>
            {
                newEntry = RealmEntry.FromDto(newEntryDto, _dbContext);
                _dbContext.Add(newEntry);
            });

            if (newEntry == null)
            {
                throw new NullReferenceException();
            }

            foreach (var sound in newEntry.Sounds)
            {
                var soundDto = newEntryDto.Sounds.FirstOrDefault(s => s.SoundId == sound.SoundId && !string.IsNullOrEmpty(s.RecordingB64));
                if (soundDto == null)
                {
                    continue;
                }

                var filePath = Path.Combine(FileService.EntrySoundsDirectory, soundDto.FileName);
                File.WriteAllText(filePath, soundDto.RecordingB64);
            }
        }
        private void HandleUpdatedEntryTranslations(EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            // Handle associated translation changes
            var existingEntryTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedEntryTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var added = updatedEntryDto.Translations.Where(t => !existingEntryTranslationIds.Contains(t.TranslationId));
            var deleted = existingEntryDto.Translations.Where(t => !updatedEntryTranslationIds.Contains(t.TranslationId));
            var updated = updatedEntryDto.Translations.Where(t => existingEntryTranslationIds.Contains(t.TranslationId) && updatedEntryTranslationIds.Contains(t.TranslationId));

            _translations.AddRange(added);
            _translations.RemoveRange(deleted.Select(t => t.TranslationId));
            _translations.UpdateRange(updated);
        }

        private void HandleUpdatedEntrySounds(EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            // Handle associated translation changes
            var existingEntrySoundIds = existingEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();
            var updatedEntrySoundIds = updatedEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();

            var added = updatedEntryDto.Sounds.Where(t => !existingEntrySoundIds.Contains(t.SoundId));
            var deleted = existingEntryDto.Sounds.Where(t => !updatedEntrySoundIds.Contains(t.SoundId));
            var updated = updatedEntryDto.Sounds.Where(t => existingEntrySoundIds.Contains(t.SoundId) && updatedEntrySoundIds.Contains(t.SoundId));

            _sounds.AddRange(added);
            _sounds.UpdateRange(updated);
            _sounds.RemoveRange(deleted.Select(t => t.SoundId));
        }

        public override void Update(EntryDto updatedEntryDto)
        {
            var existingEntry = Get(updatedEntryDto.EntryId);
            var existingEntryDto = EntryDto.FromModel(existingEntry);

            HandleUpdatedEntryTranslations(existingEntryDto, updatedEntryDto);
            HandleUpdatedEntrySounds(existingEntryDto, updatedEntryDto);

            _dbContext.Write(() =>
            {
                RealmEntry.FromDto(updatedEntryDto, _dbContext);
            });
        }
    }
}