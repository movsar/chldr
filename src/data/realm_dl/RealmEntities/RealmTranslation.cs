using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;
using Realms;

namespace realm_dl.RealmEntities;
[MapTo("Translation")]
public class RealmTranslation : RealmObject, ITranslationEntity
{
    [MapTo("Content")]
    private string _content { get; set; } = string.Empty;

    [PrimaryKey]
    public string TranslationId { get; set; }
    [Ignored] public string SourceId => Source.SourceId;
    public RealmSource Source { get; set; } = null!;
    public RealmEntry Entry { get; set; } = null!;
    public RealmUser User { get; set; } = null!;
    public string Content
    {
        get => _content;
        set
        {
            _content = value;
            RawContents = string.IsNullOrEmpty(value) ? "" : value.ToLower();
        }
    }
    public string RawContents { get; private set; }
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
        var source = context.Find<RealmSource>(translationDto.SourceId);

        translation.TranslationId = translationDto.TranslationId;
        translation.Source = source;
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

    internal static RealmTranslation FromModel(TranslationModel translationDto, RealmEntry entry, Realm context)
    {
        var user = context.Find<RealmUser>(translationDto.UserId);
        if (string.IsNullOrEmpty(translationDto.LanguageCode) || user == null || entry == null || string.IsNullOrEmpty(translationDto.EntryId))
        {
            throw new ArgumentNullException("Empty User");
        }

        var source = context.Find<RealmSource>(translationDto.SourceId);
        if (source == null)
        {
            var userSourceId = "63a816205d1af0e432fba6de";
            source = context.Find<RealmSource>(userSourceId);
        }

        // Translation
        var translation = new RealmTranslation();

        translation.TranslationId = translationDto.TranslationId;
        translation.LanguageCode = translationDto.LanguageCode;
        translation.Source = source;
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
