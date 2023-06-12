using System.ComponentModel.DataAnnotations.Schema;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.remote.Services;

namespace chldr_data.remote.SqlEntities;
[Table("Entry")]
public class SqlEntry : IEntryEntity
{
    private string? content;

    public string EntryId { get; set; }
    public string UserId { get; set; } = null!;
    public string SourceId { get; set; } = null!;
    public string? Content
    {
        get => content;
        set
        {
            content = value;
            if (string.IsNullOrEmpty(value))
            {
                RawContents = null;
            }
            else
            {
                RawContents = value?.ToLower();
            }
        }
    }
    public string? RawContents { get; private set; }
    public string? ParentEntryId { get; set; }
    public int Type { get; set; } = 0;
    public int Subtype { get; set; } = 0;
    public int Rate { get; set; } = 0;
    public string? Details { get; set; }

    public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();
    public virtual SqlSource Source { get; set; } = null!;
    public virtual SqlUser User { get; set; } = null!;
    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();

    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;

    internal static SqlEntry FromDto(EntryDto newEntryDto, SqlContext context)
    {
        // Entry
        SqlEntry? entry = context.Find<SqlEntry>(newEntryDto.EntryId);
        if (entry == null)
        {
            entry = new SqlEntry();
        }

        entry.EntryId = newEntryDto.EntryId;
        entry.UserId = newEntryDto.UserId;
        entry.SourceId = newEntryDto.SourceId!;
        entry.ParentEntryId = newEntryDto.ParentEntryId;

        entry.Type = newEntryDto.EntryType;
        entry.Subtype = newEntryDto.EntrySubtype;

        entry.Content = newEntryDto.Content;

        entry.Details = newEntryDto.Details;
        entry.Rate = newEntryDto.Rate;

        entry.CreatedAt = newEntryDto.CreatedAt;
        entry.UpdatedAt = newEntryDto.UpdatedAt;

        // Translations
        entry.Translations.Clear();
        foreach (var translationDto in newEntryDto.Translations)
        {
            var translation = (SqlTranslation)SqlTranslation.FromDto(translationDto, context);
            entry.Translations.Add(translation);
        }

        return entry;
    }
}
