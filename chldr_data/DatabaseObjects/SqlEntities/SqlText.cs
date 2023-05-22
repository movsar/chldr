using chldr_data.DatabaseObjects.DatabaseEntities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.DatabaseObjects.SqlEntities;
[Table("Text")]

public class SqlText : ITextEntity
{
    public string TextId { get; set; } = Guid.NewGuid().ToString();
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    public virtual SqlEntry Entry { get; set; } = null!;
}
