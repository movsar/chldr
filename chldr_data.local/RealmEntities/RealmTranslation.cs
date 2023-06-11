using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using Realms;

namespace chldr_data.local.RealmEntities;
[MapTo("Translation")]
public class RealmTranslation : RealmObject, ITranslationEntity
{
    [PrimaryKey]
    public string TranslationId { get; set; }
    public RealmLanguage Language { get; set; } = null!;
    public RealmEntry Entry { get; set; } = null!;
    public RealmUser User { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string RawContents { get; set; } = null!;
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

    internal static RealmTranslation FromDto(TranslationDto translationDto, Realm realm)
    {
        var language = realm.Find<RealmLanguage>(translationDto.LanguageId);
        var user = realm.Find<RealmUser>(translationDto.UserId);
        var entry = realm.Find<RealmEntry>(translationDto.EntryId);

        if (language == null || user == null || entry == null)
        {
            throw new NullReferenceException();
        }

        return new RealmTranslation()
        {
            TranslationId = translationDto.TranslationId,
            Language = language,
            Entry = entry,
            User = user,
            Content = translationDto.Content,
            RawContents = translationDto.Content.ToLower(),
            Notes = translationDto.Notes,
            Rate = translationDto.Rate,
            CreatedAt = translationDto.CreatedAt,
            UpdatedAt = translationDto.UpdatedAt,
        };
    }

    internal string GetRawContents()
    {
        return Content.ToString();
    }
}
