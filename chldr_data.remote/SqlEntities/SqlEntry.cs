using System.ComponentModel.DataAnnotations.Schema;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;

namespace chldr_data.remote.SqlEntities;
[Table("Entry")]
public class SqlEntry : IEntryEntity
{
    public string EntryId { get; set; }
    public string UserId { get; set; } = null!;
    public string SourceId { get; set; } = null!;
    public int Type { get; set; } = 0;
    public int Rate { get; set; } = 0;
    public string? ParentEntryId { get; set; }
    public string RawContents { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();
    public virtual SqlSource Source { get; set; } = null!;
    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
    public virtual SqlUser User { get; set; } = null!;
    public virtual SqlText? Text { get; set; }
    public virtual SqlPhrase? Phrase { get; set; }
    public virtual SqlWord? Word { get; set; }

    internal static SqlEntry FromDto(EntryDto newEntryDto)
    {
        // Entry
        var entry = new SqlEntry()
        {
            EntryId = newEntryDto.EntryId,
            UserId = newEntryDto.UserId,
            SourceId = newEntryDto.SourceId!,
            ParentEntryId = newEntryDto.ParentEntryId,
            Type = (int)newEntryDto.EntryType,
            Rate = newEntryDto.Rate,
            CreatedAt = newEntryDto.CreatedAt,
            UpdatedAt = newEntryDto.UpdatedAt,
        };

        // Word / phrase / text
        switch (newEntryDto.EntryType) {
            case (int)EntryType.Word:
                entry.Word = SqlWord.FromDto((WordDto)newEntryDto);
                break;
            case (int)EntryType.Phrase:
                entry.Phrase = SqlPhrase.FromDto((PhraseDto)newEntryDto);
                break;
            case (int)EntryType.Text:
                entry.Text = SqlText.FromDto((TextDto)newEntryDto);
                break;
        }

        // Translations
        foreach (var translation in newEntryDto.Translations)
        {
            entry.Translations.Add((SqlTranslation)SqlTranslation.FromDto(translation));
        }

        return entry;
    }
}
