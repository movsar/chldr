using sql_dl;
using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace sql_dl.SqlEntities;
public class SqlTranslation : ITranslationEntity
{
    private string? content;
    [Key]
    public string TranslationId { get; set; }
    public string EntryId { get; set; } = null!;
    public string UserId { get; set; } = null!;
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
    public string LanguageCode { get; set; } = null!;
    public string? Notes { get; set; }
    public int Rate { get; set; } = 0;
    public string SourceId { get; set; } = null!;
    public virtual SqlSource Source { get; set; } = null!;
    public virtual SqlEntry Entry { get; set; } = null!;
    public virtual SqlUser User { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public static SqlTranslation FromDto(TranslationDto translationDto, SqlContext context)
    {
        var user = context.Find<SqlUser>(translationDto.UserId);

        if (string.IsNullOrEmpty(translationDto.LanguageCode) || user == null || string.IsNullOrEmpty(translationDto.EntryId))
        {
            throw new NullReferenceException();
        }

        var translationEntity = context.Find<SqlTranslation>(translationDto.TranslationId);
        if (translationEntity == null)
        {
            translationEntity = new SqlTranslation();
        }

        translationEntity.SourceId = translationDto.SourceId!;
        translationEntity.EntryId = translationDto.EntryId;
        translationEntity.TranslationId = translationDto.TranslationId;
        translationEntity.LanguageCode = translationDto.LanguageCode!;
        translationEntity.UserId = translationDto.UserId;
        translationEntity.Content = translationDto.Content;
        translationEntity.RawContents = translationDto.Content.ToLower();
        translationEntity.Notes = translationDto.Notes;
        translationEntity.Rate = translationDto.Rate;
        translationEntity.CreatedAt = translationDto.CreatedAt;
        translationEntity.UpdatedAt = translationDto.UpdatedAt;

        return translationEntity;
    }

}
