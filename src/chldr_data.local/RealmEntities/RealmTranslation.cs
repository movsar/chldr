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
    public string LanguageCode { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;

    [Ignored] public string EntryId => Entry.EntryId;
    [Ignored] public string UserId => User.Id;

    internal static RealmTranslation FromDto(TranslationDto translationDto, Realm context, RealmEntry? entry = null)
    {
        var user = context.Find<RealmUser>(translationDto.UserId);

        if (string.IsNullOrEmpty(translationDto.LanguageCode) || user == null || string.IsNullOrEmpty(translationDto.EntryId))
        {
            throw new NullReferenceException();
        }

        if (entry == null)
        {
            entry = context.Find<RealmEntry>(translationDto.EntryId);
        }

        if (entry == null)
        {
            throw new NullReferenceException();
        }

        // Translation
        var translation = context.Find<RealmTranslation>(translationDto.TranslationId);
        if (translation == null)
        {
            translation = new RealmTranslation();
        }

        translation.TranslationId = translationDto.TranslationId;
        translation.LanguageCode = translationDto.LanguageCode;
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
}
