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

    public static ITranslationEntity FromDto(TranslationDto translation, IEntryEntity entry, IUserEntity user, ILanguageEntity language)
    {
        return new RealmTranslation()
        {
            TranslationId = translation.TranslationId,
            Language = language as RealmLanguage,
            Entry = entry as RealmEntry,
            User = user as RealmUser,
            Content = translation.Content,
            RawContents = translation.Content.ToLower(),
            Notes = translation.Notes,
            Rate = translation.Rate,
        };
    }

    internal string GetRawContents()
    {
        return Content.ToString();
    }
}
