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
using Microsoft.EntityFrameworkCore;

namespace chldr_data.Repositories
{
    public class RealmEntriesRepository : RealmRepository<RealmEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        private readonly ITranslationsRepository _translations;
        private readonly ISoundsRepository _sounds;

        public RealmEntriesRepository(
            Realm context,
            ExceptionHandler exceptionHandler,
            FileService fileService,
            ITranslationsRepository translationsRepository,
            ISoundsRepository soundsRepository) : base(context, exceptionHandler, fileService)
        {
            _translations = translationsRepository;
            _sounds = soundsRepository;
        }
        protected override RecordType RecordType => RecordType.Entry;
        protected override EntryModel FromEntityShortcut(RealmEntry entry)
        {
            return EntryModel.FromEntity(entry, entry.Source, entry.Translations, entry.Sounds);
        }

        public override void Add(EntryDto newEntryDto)
        {
            if (newEntryDto == null || string.IsNullOrEmpty(newEntryDto.EntryId))
            {
                throw new NullReferenceException();
            }

            RealmEntry entry = null!;

            // Insert Entry entity (with associated sound and translation entities)
            _dbContext.Write(() =>
            {
                entry = RealmEntry.FromDto(newEntryDto, _dbContext);
                _dbContext.Add(entry);
            });

            // Save audiofiles if any
            foreach (var sound in entry.Sounds)
            {
                var soundDto = newEntryDto.Sounds.FirstOrDefault(s => s.SoundId == sound.SoundId && !string.IsNullOrEmpty(s.RecordingB64));
                if (soundDto == null)
                {
                    continue;
                }

                var filePath = Path.Combine(_fileService.EntrySoundsDirectory, soundDto.FileName);
                File.WriteAllText(filePath, soundDto.RecordingB64);
            }

            InsertChangeSet(Operation.Insert, newEntryDto.UserId!, newEntryDto.EntryId);
        }


        public override void Update(EntryDto updatedEntryDto)
        {
            var existingEntry = Get(updatedEntryDto.EntryId);
            var existingEntryDto = EntryDto.FromModel(existingEntry);

            // Update associated translations and sounds
            IEntriesRepository.HandleUpdatedEntryTranslations(_translations, existingEntryDto, updatedEntryDto);
            IEntriesRepository.HandleUpdatedEntrySounds(_sounds, existingEntryDto, updatedEntryDto);

            var entryChanges = Change.GetChanges(existingEntryDto, existingEntryDto);
            if (entryChanges.Count == 0)
            {
                return;
            }

            _dbContext.Write(() =>
            {
                RealmEntry.FromDto(updatedEntryDto, _dbContext);
            });

            InsertChangeSet(Operation.Update, updatedEntryDto.UserId!, existingEntryDto.EntryId, entryChanges);
        }

        public override void Remove(string entityId)
        {
            var entry = _dbContext.Find<RealmEntry>(entityId);
            if (entry == null)
            {
                return;
            }

            var sounds = entry.Sounds.Select(s => s.SoundId).ToArray();
            var translations = entry.Translations.Select(t => t.TranslationId).ToArray();

            _sounds.RemoveRange(sounds);
            _translations.RemoveRange(translations);

            base.Remove(entityId);
        }
    }
}