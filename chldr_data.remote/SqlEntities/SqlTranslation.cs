using chldr_data.DatabaseObjects.Dtos;
using System.ComponentModel.DataAnnotations.Schema;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.remote.Services;

namespace chldr_data.remote.SqlEntities;
public class SqlTranslation : ITranslationEntity
{
    public string TranslationId { get; set; }
    public string LanguageId { get; set; } = null!;
    public string EntryId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string RawContents { get; set; } = null!;
    public string? Notes { get; set; }
    public int Rate { get; set; } = 0;
    public virtual SqlEntry Entry { get; set; } = null!;
    public virtual SqlLanguage Language { get; set; } = null!;
    public virtual SqlUser User { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    internal string GetRawContents()
    {
        return Content.ToString();
    }

    public static ITranslationEntity FromDto(TranslationDto translationDto, SqlContext context)
    {
        var translationEntity = context.Find<SqlTranslation>(translationDto.EntryId);
        if (translationEntity == null)
        {
            translationEntity = new SqlTranslation();
        }

        translationEntity.TranslationId = translationDto.TranslationId;
        translationEntity.LanguageId = translationDto.LanguageId;
        translationEntity.EntryId = translationDto.EntryId;
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
