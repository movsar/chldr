using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.Enums;
using Realms;

namespace chldr_data.local.RealmEntities;

[MapTo("Entry")]
public class RealmEntry : RealmObject, IEntryEntity
{
    [PrimaryKey] public string EntryId { get; set; }
    [Ignored] public string? SourceId => Source.SourceId;
    [Ignored] public string? UserId => User.UserId;
    public string? ParentEntryId { get; set; }
    public RealmUser User { get; set; } = null!;
    public RealmSource Source { get; set; } = null!;
    public int Type { get; set; } = 0;
    public int Subtype { get; set; } = 0;
    public int Rate { get; set; } = 0;
    public string RawContents { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public IList<RealmSound> Sounds { get; }
    public IList<RealmTranslation> Translations { get; }

    internal static RealmEntry FromDto(EntryDto entryDto, Realm realm)
    {
        var user = realm.Find<RealmUser>(entryDto.UserId);
        var source = realm.Find<RealmSource>(entryDto.SourceId);

        if (user == null || source == null)
        {
            throw new NullReferenceException();
        }

        // Entry
        RealmEntry? entry = realm.All<RealmEntry>().SingleOrDefault(e => e.EntryId.Equals(entryDto.EntryId));
        if (entry == null)
        {
            entry = new RealmEntry();
        }

        realm.Write(() =>
        {
            entry.EntryId = entryDto.EntryId;
            entry.User = user;
            entry.Source = source;
            entry.ParentEntryId = entryDto.ParentEntryId;

            entry.Content = entryDto.Content;
            entry.RawContents = entryDto.RawContents;

            entry.Type = entryDto.EntryType;
            entry.Subtype = entryDto.EntrySubtype;

            entry.Rate = entryDto.Rate;

            entry.Details = entryDto.Details;

            entry.CreatedAt = entryDto.CreatedAt;
            entry.UpdatedAt = entryDto.UpdatedAt;

            // ! This removes all translations and adds them a new, needs improvement
            entry.Translations.Clear();
            foreach (var translationDto in entryDto.Translations)
            {
                var translation = RealmTranslation.FromDto(translationDto, realm);
                entry.Translations.Add(translation);
            }
        });

        return entry;
    }

    //private void ApplyEntryTranslationChanges(EntryDto existingEntryDto, EntryDto updatedEntryDto, ITranslationsRepository translationsRepository)
    //{
    //    var existingTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
    //    var updatedTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

    //    var insertedTranslations = updatedEntryDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
    //    var deletedTranslations = existingEntryDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
    //    var updatedTranslations = updatedEntryDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

    //    foreach (var translationDto in insertedTranslations)
    //    {
    //        translationsRepository.Insert(translationDto);
    //    }

    //    foreach (var translationDto in deletedTranslations)
    //    {
    //        translationsRepository.Delete(translationDto.TranslationId);
    //    }

    //    foreach (var translationDto in updatedTranslations)
    //    {
    //        translationsRepository.Update(translationDto);
    //    }
    //}
}
