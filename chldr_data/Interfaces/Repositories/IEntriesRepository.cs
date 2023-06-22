using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface IEntriesRepository : IRepository<EntryModel, EntryDto>
    {
        public static void HandleUpdatedEntryTranslations(ITranslationsRepository translations, EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            // Handle associated translation changes
            var existingEntryTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedEntryTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var added = updatedEntryDto.Translations.Where(t => !existingEntryTranslationIds.Contains(t.TranslationId));
            var deleted = existingEntryDto.Translations.Where(t => !updatedEntryTranslationIds.Contains(t.TranslationId));
            var updated = updatedEntryDto.Translations.Where(t => existingEntryTranslationIds.Contains(t.TranslationId) && updatedEntryTranslationIds.Contains(t.TranslationId));

            translations.AddRange(added);
            translations.RemoveRange(deleted.Select(t => t.TranslationId));
            translations.UpdateRange(updated);
        }

        public static void HandleUpdatedEntrySounds(ISoundsRepository sounds, EntryDto existingEntryDto, EntryDto updatedEntryDto)
        {
            // Handle associated translation changes
            var existingEntrySoundIds = existingEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();
            var updatedEntrySoundIds = updatedEntryDto.Sounds.Select(t => t.SoundId).ToHashSet();

            var added = updatedEntryDto.Sounds.Where(t => !existingEntrySoundIds.Contains(t.SoundId));
            var deleted = existingEntryDto.Sounds.Where(t => !updatedEntrySoundIds.Contains(t.SoundId));
            var updated = updatedEntryDto.Sounds.Where(t => existingEntrySoundIds.Contains(t.SoundId) && updatedEntrySoundIds.Contains(t.SoundId));

            sounds.AddRange(added);
            sounds.UpdateRange(updated);
            sounds.RemoveRange(deleted.Select(t => t.SoundId));
        }
    }
}
