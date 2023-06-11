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
    public int Rate { get; set; } = 0;
    public string RawContents { get; set; } = string.Empty;
    public RealmText? Text { get; set; }
    public RealmPhrase? Phrase { get; set; }
    public RealmWord? Word { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public IList<RealmSound> Sounds { get; }
    public IList<RealmTranslation> Translations { get; }

    internal static RealmEntry FromDto(EntryDto newEntryDto, IUserEntity user, ISourceEntity source)
    {
        // Entry
        var entry = new RealmEntry()
        {
            EntryId = newEntryDto.EntryId,
            User = (RealmUser)user,
            Source =(RealmSource)source,
            ParentEntryId = newEntryDto.ParentEntryId,
            Type = newEntryDto.EntryType,
            Rate = newEntryDto.Rate,
            CreatedAt = newEntryDto.CreatedAt,
            UpdatedAt = newEntryDto.UpdatedAt,
        };

        // Word / phrase / text
        switch (newEntryDto.EntryType)
        {
            case (int)EntryType.Word:
                var wordDto = newEntryDto as WordDto;
                entry.Word = new RealmWord()
                {
                    Entry = entry,
                    WordId = wordDto.WordId,
                    Content = wordDto.Content,
                    PartOfSpeech = (int)wordDto.PartOfSpeech,
                };
                break;

            case (int)EntryType.Phrase:
                var phraseDto = newEntryDto as PhraseDto;
                entry.Phrase = new RealmPhrase()
                {
                    Entry = entry,
                    PhraseId = phraseDto.PhraseId,
                    Content = phraseDto.Content,
                };
                break;

            case (int)EntryType.Text:
                var textDto = newEntryDto as TextDto;
                entry.Text = new RealmText()
                {
                    Entry = entry,
                    TextId = textDto.TextId,
                    Content = textDto.Content,
                };
                break;

        }

        // Translations
        entry.Translations.Clear();
        foreach (var translationDto in newEntryDto.Translations)
        {
            var translation = (RealmTranslation)RealmTranslation.FromDto(translationDto, entry, user, translationDto.Language);
            entry.Translations.Add(translation);
        }

        return entry;
    }

}
