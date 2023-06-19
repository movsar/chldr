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


        public override void Update(EntryDto updatedEntryDto)
        {
            var existingEntry = Get(updatedEntryDto.EntryId);
            var existingEntryDto = EntryDto.FromModel(existingEntry);

            IEntriesRepository.HandleUpdatedEntryTranslations(_translations, existingEntryDto, updatedEntryDto);
            IEntriesRepository.HandleUpdatedEntrySounds(_sounds, existingEntryDto, updatedEntryDto);

            _dbContext.Write(() =>
            {
                RealmEntry.FromDto(updatedEntryDto, _dbContext);
            });
        }

        public override void Remove(string entityId)
        {
            base.Remove(entityId);

            var sounds = _dbContext.All<RealmSound>().Where(s => s.EntryId.Equals(entityId));
            var translations = _dbContext.All<RealmTranslation>().Where(t => t.EntryId.Equals(entityId));

            _sounds.RemoveRange(sounds.Select(s => s.SoundId));
            _translations.RemoveRange(translations.Select(t => t.TranslationId));
        }
    }
}