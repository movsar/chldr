using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.remote.SqlEntities;


[Table("Translation")]
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
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    internal string GetRawContents()
    {
        return Content.ToString();
    }

    public static ITranslationEntity FromDto(TranslationDto translation)
    {
        return new SqlTranslation()
        {
            TranslationId = translation.TranslationId,
            LanguageId = translation.LanguageId,
            EntryId = translation.EntryId,
            UserId = translation.UserId,
            Content = translation.Content,
            RawContents = translation.Content.ToLower(),
            Notes = translation.Notes,
            Rate = translation.Rate,
        };
    }

}
