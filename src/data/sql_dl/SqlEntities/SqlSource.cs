using domain.DatabaseObjects.Dtos;
using System.ComponentModel.DataAnnotations.Schema;
using domain.DatabaseObjects.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace sql_dl.SqlEntities;

[Table("Source")]
public class SqlSource : ISourceEntity
{
    [Key]
    public string SourceId { get; set; } 
    public string? UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
    public virtual SqlUser? User { get; set; }
    public static ISourceEntity FromDto(SourceDto source)
    {
        return new SqlSource()
        {
            SourceId = source.SourceId,
            UserId = source.UserId,
            Name = source.Name,
            Notes = source.Notes,
        };
    }
}
