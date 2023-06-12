using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using Realms;

namespace chldr_data.local.RealmEntities;
[MapTo("Translation")]
public class RealmTranslation : RealmObject, ITranslationEntity
{
    [MapTo("Content")]
    private string? _content { get; set; }

    [PrimaryKey]
    public string TranslationId { get; set; }
    public RealmLanguage Language { get; set; } = null!;
    public RealmEntry Entry { get; set; } = null!;
    public RealmUser User { get; set; } = null!;
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
    public string? Notes { get; set; }
    public int Rate { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;

    [Ignored]
    public string EntryId => Entry.EntryId;
    [Ignored]
    public string UserId => User.UserId;
    [Ignored]
    public string LanguageId => Language.LanguageId;

    internal static RealmTranslation FromDto(TranslationDto translationDto, RealmEntry entry, Realm realm)
    {
        var language = realm.Find<RealmLanguage>(translationDto.LanguageId);
        var user = realm.Find<RealmUser>(translationDto.UserId);

        if (language == null || user == null || entry == null)
        {
            throw new NullReferenceException();
        }
        
        // Translation
        var translation = realm.Find<RealmTranslation>(translationDto.EntryId);
        translation = realm.All<RealmTranslation>().SingleOrDefault(t => t.TranslationId.Equals(translationDto.TranslationId));

        if (translation == null)
        {
            translation = new RealmTranslation();
        }

        translation.TranslationId = translationDto.TranslationId;
        translation.Language = language;
        translation.Entry = entry;
        translation.User = user;
        translation.Content = translationDto.Content;
        translation.RawContents = translationDto.Content.ToLower();
        translation.Notes = translationDto.Notes;
        translation.Rate = translationDto.Rate;
        translation.CreatedAt = translationDto.CreatedAt;
        translation.UpdatedAt = translationDto.UpdatedAt;

        return translation;
    }

    internal string GetRawContents()
    {
        return Content.ToString();
    }
}
