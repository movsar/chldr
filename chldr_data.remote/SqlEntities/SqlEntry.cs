using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
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
        entry.Translations.Clear();
        foreach (var translationDto in newEntryDto.Translations)
        {
            var translation = (SqlTranslation)SqlTranslation.FromDto(translationDto);
            entry.Translations.Add(translation);
        }

        return entry;
    }
}
