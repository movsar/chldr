using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.DatabaseObjects.SqlEntities;

[Table("Source")]
public class SqlSource : ISourceEntity
{
    public string SourceId { get; set; } 
    public string? UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<SqlEntry> Entries { get; set; } = new List<SqlEntry>();
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
