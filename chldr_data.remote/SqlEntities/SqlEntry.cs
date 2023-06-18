using System.ComponentModel.DataAnnotations.Schema;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.remote.Services;
using Realms;

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
            RawContents = string.IsNullOrEmpty(value) ? null : value.ToLower();
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

    internal static SqlEntry FromDto(EntryDto entryDto, SqlContext context)
    {
        // Entry
        SqlEntry? entry = context.Find<SqlEntry>(entryDto.EntryId);
        if (entry == null)
        {
            entry = new SqlEntry();
        }

        entry.EntryId = entryDto.EntryId;
        entry.UserId = entryDto.UserId;
        entry.SourceId = entryDto.SourceId!;
        entry.ParentEntryId = entryDto.ParentEntryId;

        entry.Type = entryDto.EntryType;
        entry.Subtype = entryDto.EntrySubtype;

        entry.Content = entryDto.Content;

        entry.Details = entryDto.Details;
        entry.Rate = entryDto.Rate;

        entry.CreatedAt = entryDto.CreatedAt;
        entry.UpdatedAt = entryDto.UpdatedAt;

        // Sounds
        foreach (var soundDto in entryDto.Sounds)
        {
            soundDto.EntryId = entry.EntryId;

            var sound = SqlSound.FromDto(soundDto, context);
            entry.Sounds.Add(sound);
        }

        // Translations
        entry.Translations.Clear();
        foreach (var translationDto in entryDto.Translations)
        {
            // If entry didn't exist, this will map its Id to translations
            translationDto.EntryId = entry.EntryId;

            var translation = SqlTranslation.FromDto(translationDto, context);
            entry.Translations.Add(translation);
        }

        return entry;
    }
}
