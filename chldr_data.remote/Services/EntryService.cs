using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.remote.Services
{
    internal class EntryService
    {
        private readonly IEntriesRepository _entries;
        private readonly ITranslationsRepository _translations;
        private readonly ISoundsRepository _sounds;
        private readonly IChangeSetsRepository _changeSets;
        private readonly ExceptionHandler _exceptionHandler;

        internal EntryService(ExceptionHandler exceptionHandler,
                              IEntriesRepository entries,
                              ITranslationsRepository translations,
                              ISoundsRepository sounds,
                              IChangeSetsRepository changeSets
            )
        {
            _entries = entries;
            _translations = translations;
            _sounds = sounds;
            _changeSets = changeSets;
            _exceptionHandler = exceptionHandler;
        }
        public RequestResult AddEntry(string userId, EntryDto entryDto)
        {
            try
            {
                // Process added entry
                _entries.Add(entryDto);

                var entryChangeSet = ChangeSetDto.Create(Operation.Insert, userId, RecordType.Entry, entryDto.EntryId);
                _changeSets.Add(entryChangeSet);

                // Process added translations
                foreach (var translation in entryDto.Translations)
                {
                    _translations.Add(translation);

                    var translationChangeSet = ChangeSetDto.Create(Operation.Insert, userId, RecordType.Translation, translation.TranslationId);
                    _changeSets.Add(translationChangeSet);
                }

                // Process added sounds
                foreach (var sound in entryDto.Sounds)
                {
                    _sounds.Add(sound);

                    var translationChangeSet = ChangeSetDto.Create(Operation.Insert, userId, RecordType.Sound, sound.SoundId);
                    _changeSets.Add(translationChangeSet);
                }

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(new
                    {
                        entryDto.CreatedAt
                    })
                };
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogError(ex);
            }

            return new RequestResult() { Success = false };
        }

        public RequestResult UpdateEntry(string userId, EntryDto updatedEntryDto)
        {
            var resultingChangeSetDtos = new List<ChangeSetDto>();

            try
            {
                var existingEntry = _entries.Get(updatedEntryDto.EntryId);
                var existingEntryDto = EntryDto.FromModel(existingEntry);

                // Update entry
                var entryChanges = Change.GetChanges(updatedEntryDto, existingEntryDto);
                if (entryChanges.Count != 0)
                {
                    _entries.Update(updatedEntryDto);

                    var entryChangeSetDto = ChangeSetDto.Create(Operation.Update, userId, RecordType.Entry, updatedEntryDto.EntryId, entryChanges);
                    resultingChangeSetDtos.Add(entryChangeSetDto);
                }

                // Add / Remove / Update translations
                var translationChangeSets = ProcessTranslationsForEntryUpdate(userId, existingEntryDto, updatedEntryDto);
                resultingChangeSetDtos.AddRange(translationChangeSets);

                // Add / Remove / Update sounds
                var soundChangeSets = ProcessSoundsForEntryUpdate(userId, existingEntryDto, updatedEntryDto);
                resultingChangeSetDtos.AddRange(soundChangeSets);

                // Insert changesets
                _changeSets.AddRange(resultingChangeSetDtos);

                return new RequestResult() { Success = true };
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogError(ex);
            }

            return new RequestResult() { Success = false };
        }

        public RequestResult RemoveEntry(string userId, string entryId)
        {
            try
            {
                var entry = _entries.Get(entryId);
                var soundIds = entry.Sounds.Select(s => s.SoundId).ToArray();
                var translationIds = entry.Translations.Select(t => t.TranslationId).ToArray();

                // Process removed translations
                foreach (var translationId in translationIds)
                {
                    _translations.Remove(translationId);

                    var translationChangeSet = ChangeSetDto.Create(Operation.Delete, userId, RecordType.Translation, translationId);
                    _changeSets.Add(translationChangeSet);
                }

                // Process removed sounds
                foreach (var soundId in soundIds)
                {
                    _sounds.Remove(soundId);

                    var translationChangeSet = ChangeSetDto.Create(Operation.Delete, userId, RecordType.Sound, soundId);
                    _changeSets.Add(translationChangeSet);
                }

                // Process removed entry
                _entries.Remove(entryId);

                var changeSet = ChangeSetDto.Create(Operation.Delete, userId, RecordType.Entry, entryId);
                _changeSets.Add(changeSet);

                return new RequestResult() { Success = true };
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogError(ex);
            }

            return new RequestResult() { Success = false };
        }

        private IEnumerable<ChangeSetDto> ProcessSoundsForEntryUpdate(string userId, EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            var changeSets = new List<ChangeSetDto>();

            var existingEntrySoundIds = existingEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();
            var updatedEntrySoundIds = updatedEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();

            var added = updatedEntryDto.Sounds.Where(t => !existingEntrySoundIds.Contains(t.SoundId));
            var deleted = existingEntryDto.Sounds.Where(t => !updatedEntrySoundIds.Contains(t.SoundId));
            var updated = updatedEntryDto.Sounds.Where(t => existingEntrySoundIds.Contains(t.SoundId) && updatedEntrySoundIds.Contains(t.SoundId));

            // Process inserted translations
            foreach (var sound in added)
            {
                _sounds.Add(sound);

                var changeSet = ChangeSetDto.Create(Operation.Insert, userId, RecordType.Sound, sound.SoundId);
                changeSets.Add(changeSet);
            }

            // Process removed translations
            foreach (var sound in deleted)
            {
                _sounds.Remove(sound.SoundId);

                var changeSet = ChangeSetDto.Create(Operation.Delete, userId, RecordType.Sound, sound.SoundId);
                changeSets.Add(changeSet);
            }

            // Process updated translations
            foreach (var sound in updated)
            {
                var existingDto = existingEntryDto.Sounds.First(t => t.SoundId.Equals(sound.SoundId));

                var changes = Change.GetChanges(sound, existingDto);
                if (changes.Count == 0)
                {
                    continue;
                }

                _sounds.Update(sound);

                var changeSet = ChangeSetDto.Create(Operation.Update, userId, RecordType.Sound, sound.SoundId, changes);
                changeSets.Add(changeSet);
            }

            return changeSets;
        }
        private IEnumerable<ChangeSetDto> ProcessTranslationsForEntryUpdate(string userId, EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            var changeSets = new List<ChangeSetDto>();

            var existingEntryTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedEntryTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var added = updatedEntryDto.Translations.Where(t => !existingEntryTranslationIds.Contains(t.TranslationId));
            var deleted = existingEntryDto.Translations.Where(t => !updatedEntryTranslationIds.Contains(t.TranslationId));
            var updated = updatedEntryDto.Translations.Where(t => existingEntryTranslationIds.Contains(t.TranslationId) && updatedEntryTranslationIds.Contains(t.TranslationId));

            // Process inserted translations
            foreach (var translation in added)
            {
                _translations.Add(translation);

                var changeSet = ChangeSetDto.Create(Operation.Insert, userId, RecordType.Translation, translation.TranslationId);
                changeSets.Add(changeSet);
            }

            // Process removed translations
            foreach (var translation in deleted)
            {
                _translations.Remove(translation.TranslationId);

                var changeSet = ChangeSetDto.Create(Operation.Delete, userId, RecordType.Translation, translation.TranslationId);
                changeSets.Add(changeSet);
            }

            // Process updated translations
            foreach (var translation in updated)
            {
                var existingTranslationDto = existingEntryDto.Translations.First(t => t.TranslationId.Equals(translation.TranslationId));

                var changes = Change.GetChanges(translation, existingTranslationDto);
                if (changes.Count == 0)
                {
                    continue;
                }

                _translations.Update(translation);

                var changeSet = ChangeSetDto.Create(Operation.Update, userId, RecordType.Translation, translation.TranslationId, changes);
                changeSets.Add(changeSet);
            }
            return changeSets;
        }

    }
}
