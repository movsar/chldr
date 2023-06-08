using chldr_data.DatabaseObjects.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.remote.SqlEntities;


[Table("Language")]
public class SqlLanguage : ILanguageEntity
{
    public string LanguageId { get; set; }
    public string? UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<SqlTranslation> Translations { get; set; }
    public virtual SqlUser? User { get; set; }
}
