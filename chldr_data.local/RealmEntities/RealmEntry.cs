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
        // Entry
        RealmEntry? entry = realm.All<RealmEntry>().SingleOrDefault(e => e.EntryId.Equals(e.EntryId));
        if (entry == null)
        {
            entry = new RealmEntry();
        }

        entry.EntryId = entryDto.EntryId;
        entry.User = realm.Find<RealmUser>(entryDto.UserId)!;
        entry.Source = realm.Find<RealmSource>(entryDto.UserId)!;
        entry.ParentEntryId = entryDto.ParentEntryId;

        entry.Content = entryDto.Content;
        entry.RawContents = entryDto.RawContents;

        entry.Type = entryDto.EntryType;
        entry.Subtype = entryDto.EntrySubtype;

        entry.Rate = entryDto.Rate;

        entry.Details = entryDto.Details;   

        entry.CreatedAt = entryDto.CreatedAt;
        entry.UpdatedAt = entryDto.UpdatedAt;

        // Translations
        entry.Translations.Clear();
        foreach (var translationDto in entryDto.Translations)
        {
            var translation = RealmTranslation.FromDto(translationDto, realm);
            entry.Translations.Add(translation);
        }

        return entry;
    }

}
