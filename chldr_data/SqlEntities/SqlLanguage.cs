using chldr_data.Interfaces;
using chldr_data.Interfaces.DatabaseEntities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;


[Table("Language")]
public class SqlLanguage : ILanguageEntity
{
    public string LanguageId { get; set; } = Guid.NewGuid().ToString();
    public string? UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<ITranslationEntity> Translations { get; set; }
    public virtual IUserEntity? User { get; set; }
}
