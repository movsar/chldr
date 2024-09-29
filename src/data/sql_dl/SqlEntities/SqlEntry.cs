using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using domain;
using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
using sql_dl;
using domain.Exceptions;
using domain.Enums;

namespace sql_dl.SqlEntities;
[Table("Entry")]
public class SqlEntry : IEntryEntity
{
    private string? content;
    [Key]
    public string EntryId { get; set; }
    public string? ParentEntryId { get; set; }
    public string UserId { get; set; } = null!;
    public int Type { get; set; } = 0;
    public int Rate { get; set; } = 0;
    public int Subtype { get; set; } = 0;
    public string? Content
    {
        get => content;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidArgumentsException();
            }

            content = value;
            RawContents = value.ToLower();
        }
    }
    public string RawContents { get; private set; }
    public string? Details { get; set; }
    public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();
    public virtual SqlUser User { get; set; } = null!;
    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public static bool ValidateParentId(EntryDto entryDto, SqlContext context)
    {
        if (string.IsNullOrEmpty(entryDto.ParentEntryId))
        {
            return false;
        }

        if (entryDto.ParentEntryId == entryDto.EntryId)
        {
            // Self-linking is not allowed
            return false;
        }

        var isCircularReference = IsCircularReference(entryDto.EntryId, entryDto.ParentEntryId, context);
        if (isCircularReference)
        {
            // Circular linking is not allowed
            return false;
        }

        return true;
    }

    private static bool IsCircularReference(string entryId, string parentEntryId, SqlContext context)
    {
        var currentEntry = context.Entries.Find(entryId);
        while (currentEntry != null)
        {
            if (currentEntry.EntryId == parentEntryId)
            {
                // Circular reference detected
                return true;
            }

            currentEntry = context.Entries.Find(currentEntry.ParentEntryId);
        }

        return false;
    }
    internal static SqlEntry FromDto(EntryDto entryDto, SqlContext context)
    {
        // Entry
        SqlEntry? entry = context.Find<SqlEntry>(entryDto.EntryId);
        if (entry == null)
        {
            entry = new SqlEntry();
        }

        if (entryDto.Type == (int)EntryType.Word && !string.IsNullOrEmpty(entryDto.ParentEntryId) && ValidateParentId(entryDto, context) == false)
        {
            throw new Exception("Error:Invalid_parent_id");
        }

        entry.EntryId = entryDto.EntryId;
        entry.ParentEntryId = entryDto.ParentEntryId;
        entry.UserId = entryDto.UserId;

        entry.Type = entryDto.Type;
        entry.Subtype = entryDto.Subtype;
        entry.Details = entryDto.Details;

        entry.Content = entryDto.Content;
        entry.Rate = entryDto.Rate;

        entry.CreatedAt = entryDto.CreatedAt;
        entry.UpdatedAt = entryDto.UpdatedAt;

        // Sounds
        entry.Sounds.Clear();
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
