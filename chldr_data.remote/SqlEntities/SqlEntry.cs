using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Enums.WordDetails;

namespace chldr_data.remote.SqlEntities;
[Table("Entry")]
public class SqlEntry : IEntryEntity
{
    public string EntryId { get; set; }
    public string UserId { get; set; } = null!;
    public string SourceId { get; set; } = null!;
    public string RawContents { get; set; }
    public string? ParentEntryId { get; set; }
    public int Type { get; set; } = 0;
    public int Rate { get; set; } = 0;

    public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();
    public virtual SqlSource Source { get; set; } = null!;
    public virtual SqlUser User { get; set; } = null!;

    public virtual SqlText? Text { get; set; }
    public virtual SqlPhrase? Phrase { get; set; }
    public virtual SqlWord? Word { get; set; }

    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();

    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;

    internal static SqlEntry FromDto(EntryDto newEntryDto)
    {
        // Entry
        var entry = new SqlEntry()
        {
            EntryId = newEntryDto.EntryId,
            UserId = newEntryDto.UserId,
            SourceId = newEntryDto.SourceId!,
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
                entry.Word = new SqlWord()
                {
                    Entry = entry,
                    EntryId = wordDto.EntryId,
                    WordId = wordDto.WordId,
                    Content = wordDto.Content,
                    PartOfSpeech = (int)wordDto.PartOfSpeech,
                };
                break;

            case (int)EntryType.Phrase:
                var phraseDto = newEntryDto as PhraseDto;
                entry.Phrase = new SqlPhrase()
                {
                    Entry = entry,
                    EntryId = phraseDto.EntryId,
                    PhraseId = phraseDto.PhraseId,
                    Content = phraseDto.Content,
                };
                break;

            case (int)EntryType.Text:
                var textDto = newEntryDto as TextDto;
                entry.Text = new SqlText()
                {
                    Entry = entry,
                    EntryId = textDto.EntryId,
                    TextId = textDto.TextId,
                    Content = textDto.Content,
                };
                break;

        }

        // Translations
        entry.Translations.Clear();
        foreach (var translationDto in newEntryDto.Translations)
        {
            var translation = (SqlTranslation)SqlTranslation.FromDto(translationDto);
            entry.Translations.Add(translation);
        }

        return entry;
    }
}
