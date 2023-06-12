using chldr_data.DatabaseObjects.Dtos;
using System.ComponentModel.DataAnnotations.Schema;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.remote.Services;
using Realms;

namespace chldr_data.remote.SqlEntities;
public class SqlTranslation : ITranslationEntity
{
    private string? content;
    public string TranslationId { get; set; }
    public string LanguageId { get; set; } = null!;
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
    public string? Notes { get; set; }
    public int Rate { get; set; } = 0;
    public virtual SqlEntry Entry { get; set; } = null!;
    public virtual SqlLanguage Language { get; set; } = null!;
    public virtual SqlUser User { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public static SqlTranslation FromDto(TranslationDto translationDto, SqlContext context)
    {
        var language = context.Find<SqlLanguage>(translationDto.LanguageId);
        var user = context.Find<SqlUser>(translationDto.UserId);

        if (language == null || user == null || string.IsNullOrEmpty(translationDto.EntryId))
        {
            throw new NullReferenceException();
        }

        var translationEntity = context.Find<SqlTranslation>(translationDto.TranslationId);
        if (translationEntity == null)
        {
            translationEntity = new SqlTranslation();
        }

        translationEntity.EntryId = translationDto.EntryId;
        translationEntity.TranslationId = translationDto.TranslationId;
        translationEntity.LanguageId = translationDto.LanguageId!;
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
