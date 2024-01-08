using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using Newtonsoft.Json;
using Realms;

namespace chldr_data.realm.RealmEntities;

[MapTo("Entry")]
public class RealmEntry : RealmObject, IEntryEntity
{
    [MapTo("Content")]
    private string? _content { get; set; }

    [PrimaryKey] public string EntryId { get; set; }
    [Ignored] public string? SourceId => Source.SourceId;
    [Ignored] public string? UserId => User.Id;
    public string? ParentEntryId { get; set; }
    public RealmUser User { get; set; } = null!;
    public RealmSource Source { get; set; } = null!;
    public int Type { get; set; } = 0;
    public int Subtype { get; set; } = 0;
    public int Rate { get; set; } = 0;
    public string Content
    {
        get => _content;
        set
        {
            _content = value;
            RawContents = string.IsNullOrEmpty(value) ? null : value.ToLower();
        }
    }
    public string? RawContents { get; private set; }
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

        entry.EntryId = entryDto.EntryId;
        entry.User = user;
        entry.Source = source;
        entry.ParentEntryId = entryDto.ParentEntryId;

        entry.Content = entryDto.Content;

        entry.Type = entryDto.Type;
        entry.Subtype = entryDto.Subtype;

        entry.Rate = entryDto.Rate;

        entry.Details = entryDto.Details;

        entry.CreatedAt = entryDto.CreatedAt;
        entry.UpdatedAt = entryDto.UpdatedAt;

        // Sounds
        entry.Sounds.Clear();
        foreach (var soundDto in entryDto.Sounds)
        {
            soundDto.EntryId = entry.EntryId;

            var sound = RealmSound.FromDto(soundDto, realm, entry);
            entry.Sounds.Add(sound);
        }

        // Translations
        entry.Translations.Clear();
        foreach (var translationDto in entryDto.Translations)
        {
            // If entry didn't exist, this will map its Id to translations
            translationDto.EntryId = entry.EntryId;

            var translation = RealmTranslation.FromDto(translationDto, realm, entry);
            entry.Translations.Add(translation);
        }

        return entry;
    }

    internal static RealmEntry FromModel(EntryModel entryModel, RealmUser user, RealmSource source)
    {
        if (string.IsNullOrEmpty(entryModel.UserId) || string.IsNullOrEmpty(entryModel.SourceId))
        {
            throw new NullReferenceException();
        }

        // Entry
        var entry = new RealmEntry();

        entry.EntryId = entryModel.EntryId;
        entry.User = user;
        entry.Source = source;
        entry.ParentEntryId = entryModel.ParentEntryId;

        entry.Content = entryModel.Content;

        entry.Type = (int)entryModel.Type;
        entry.Subtype = entryModel.Subtype;

        entry.Rate = entryModel.Rate;

        entry.Details = JsonConvert.SerializeObject(entryModel.Details);

        entry.CreatedAt = entryModel.CreatedAt;
        entry.UpdatedAt = entryModel.UpdatedAt;

        // Sounds
        entry.Sounds.Clear();
        foreach (var soundModel in entryModel.Sounds)
        {
            soundModel.EntryId = entry.EntryId;

            var sound = RealmSound.FromModel(soundModel, user, entry);
            entry.Sounds.Add(sound);
        }

        // Translations
        entry.Translations.Clear();
        foreach (var translationDto in entryModel.Translations)
        {
            // If entry didn't exist, this will map its Id to translations
            translationDto.EntryId = entry.EntryId;

            var translation = RealmTranslation.FromModel(translationDto, user, entry);
            entry.Translations.Add(translation);
        }

        // SubEntries
        foreach (var subEntry in entryModel.SubEntries)
        {

        }

        return entry;
    }
}
