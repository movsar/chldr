using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Language")]
public partial class SqlLanguage
{
    public string LanguageId { get; set; } = Guid.NewGuid().ToString();
    public string? UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    [JsonIgnore]
    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
    [JsonIgnore]
    public virtual SqlUser? User { get; set; }
}
