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
        entry.Type = entryDto.EntryType;
        entry.Rate = entryDto.Rate;
        entry.CreatedAt = entryDto.CreatedAt;
        entry.UpdatedAt = entryDto.UpdatedAt;

        // Word / phrase / text
        switch (entryDto.EntryType)
        {
            case (int)EntryType.Word:
                var wordDto = entryDto as WordDto;
                entry.Word = new RealmWord()
                {
                    Entry = entry,
                    WordId = wordDto.WordId,
                    Content = wordDto.Content,
                    PartOfSpeech = (int)wordDto.PartOfSpeech,
                };
                break;

            case (int)EntryType.Phrase:
                var phraseDto = entryDto as PhraseDto;
                entry.Phrase = new RealmPhrase()
                {
                    Entry = entry,
                    PhraseId = phraseDto.PhraseId,
                    Content = phraseDto.Content,
                };
                break;

            case (int)EntryType.Text:
                var textDto = entryDto as TextDto;
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
        foreach (var translationDto in entryDto.Translations)
        {
            var translation = RealmTranslation.FromDto(translationDto, realm);
            entry.Translations.Add(translation);
        }

        return entry;
    }

}
